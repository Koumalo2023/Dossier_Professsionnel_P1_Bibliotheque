using api.Helpers;
using api.Helpers.Enums;
using api.Models;
using api.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
namespace api.Services
{
    /// <summary>
    /// Implémentation du service pour la gestion des notifications
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initialise une nouvelle instance du service des notifications
        /// </summary>
        /// <param name="notificationRepository">Repository des notifications</param>
        /// <param name="mapper">Mapper AutoMapper</param>
        /// <param name="userManager">Gestionnaire d'utilisateurs</param>
        public NotificationService(
            INotificationRepository notificationRepository,
            IMapper mapper,
            UserManager<ApplicationUser> userManager)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Récupère les notifications d'un utilisateur avec filtrage optionnel
        /// </summary>
        public async Task<ServiceResponse<PagedResult<NotificationDto>>> GetUserNotificationsAsync(Guid userId, bool? read = null, string? type = null, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                var pagedResult = await _notificationRepository.GetUserNotificationsAsync(userId, read, type, pageNumber, pageSize);
                var notificationDtos = _mapper.Map<List<NotificationDto>>(pagedResult.Items);
                
                var result = new PagedResult<NotificationDto>(
                    notificationDtos,
                    pagedResult.TotalCount,
                    pagedResult.CurrentPage,
                    pagedResult.PageSize
                );

                return new ServiceResponse<PagedResult<NotificationDto>>
                {
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PagedResult<NotificationDto>>
                {
                    Success = false,
                    Message = $"Erreur lors de la récupération des notifications: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Marque une notification comme lue
        /// </summary>
        public async Task<ServiceResponse<bool>> MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            try
            {
                // Vérifier que la notification appartient à l'utilisateur
                var belongsToUser = await _notificationRepository.BelongsToUserAsync(notificationId, userId);
                if (!belongsToUser)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Notification non trouvée ou accès non autorisé"
                    };
                }

                var success = await _notificationRepository.MarkAsReadAsync(notificationId);
                if (!success)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Notification non trouvée"
                    };
                }

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Notification marquée comme lue"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Erreur lors du marquage de la notification: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Marque toutes les notifications d'un utilisateur comme lues
        /// </summary>
        public async Task<ServiceResponse<int>> MarkAllAsReadAsync(Guid userId)
        {
            try
            {
                var count = await _notificationRepository.MarkAllAsReadAsync(userId);
                return new ServiceResponse<int>
                {
                    Success = true,
                    Data = count,
                    Message = $"{count} notification(s) marquée(s) comme lue(s)"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>
                {
                    Success = false,
                    Message = $"Erreur lors du marquage des notifications: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Crée une notification personnalisée (réservé aux managers et admins)
        /// </summary>
        public async Task<ServiceResponse<NotificationDto>> CreateCustomNotificationAsync(CreateNotificationDto createDto, Guid currentUserId)
        {
            try
            {
                // Vérifier les permissions
                var canManage = await CanManageNotificationsAsync(currentUserId);
                if (!canManage)
                {
                    return new ServiceResponse<NotificationDto>
                    {
                        Success = false,
                        Message = "Autorisation insuffisante pour créer des notifications"
                    };
                }

                // Vérifier que l'utilisateur cible existe
                var targetUser = await _userManager.FindByIdAsync(createDto.UserId.ToString());
                if (targetUser == null)
                {
                    return new ServiceResponse<NotificationDto>
                    {
                        Success = false,
                        Message = "Utilisateur cible non trouvé"
                    };
                }

                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = createDto.UserId,
                    Message = createDto.Message,
                    Type = createDto.Type,
                    Read = false,
                    CreatedAt = DateTime.UtcNow
                };

                var createdNotification = await _notificationRepository.CreateAsync(notification);
                var notificationDto = _mapper.Map<NotificationDto>(createdNotification);

                return new ServiceResponse<NotificationDto>
                {
                    Success = true,
                    Data = notificationDto,
                    Message = "Notification créée avec succès"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<NotificationDto>
                {
                    Success = false,
                    Message = $"Erreur lors de la création de la notification: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Récupère le nombre de notifications non lues pour un utilisateur
        /// </summary>
        public async Task<ServiceResponse<UnreadNotificationsResponse>> GetUnreadCountAsync(Guid userId)
        {
            try
            {
                var stats = await _notificationRepository.GetUnreadCountAsync(userId);
                var response = new UnreadNotificationsResponse
                {
                    Count = stats.Count,
                    HighPriority = stats.HighPriority
                };

                return new ServiceResponse<UnreadNotificationsResponse>
                {
                    Success = true,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UnreadNotificationsResponse>
                {
                    Success = false,
                    Message = $"Erreur lors de la récupération du compte des notifications: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Vérifie si l'utilisateur a la permission de gérer les notifications
        /// </summary>
        public async Task<bool> CanManageNotificationsAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var roles = await _userManager.GetRolesAsync(user);
            return roles.Any(r => r == RoleTypes.Manager || r == RoleTypes.Admin);
        }
    }
}