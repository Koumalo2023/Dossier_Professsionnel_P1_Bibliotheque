using api.Models;
using api.Helpers;
using api.Helpers.Enums;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des notifications
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur des notifications
        /// </summary>
        /// <param name="notificationService">Service de gestion des notifications</param>
        /// <param name="logger">Logger pour le suivi des activités</param>
        public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Récupère les notifications de l'utilisateur connecté avec filtrage optionnel
        /// </summary>
        /// <param name="read">Filtrer par statut de lecture (true = lues, false = non lues, null = tous)</param>
        /// <param name="type">Filtrer par type de notification (REMINDER, INFO, WARNING, etc.)</param>
        /// <param name="pageNumber">Numéro de page (défaut: 1)</param>
        /// <param name="pageSize">Taille de la page (défaut: 20)</param>
        /// <returns>Liste paginée des notifications</returns>
        [HttpGet]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetNotifications(
            [FromQuery] bool? read = null,
            [FromQuery] string? type = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _notificationService.GetUserNotificationsAsync(userId, read, type, pageNumber, pageSize);
                
                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des notifications pour l'utilisateur {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Marque une notification comme lue
        /// </summary>
        /// <param name="id">Identifiant de la notification</param>
        /// <returns>Résultat de l'opération</returns>
        [HttpPut("{id}/read")]
        public async Task<ActionResult> MarkAsRead(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _notificationService.MarkAsReadAsync(id, userId);
                
                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return Ok(new { message = "Notification marquée comme lue" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage de la notification {NotificationId} comme lue pour l'utilisateur {UserId}", id, GetCurrentUserId());
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Marque toutes les notifications de l'utilisateur comme lues
        /// </summary>
        /// <returns>Nombre de notifications mises à jour</returns>
        [HttpPut("mark-all-read")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _notificationService.MarkAllAsReadAsync(userId);
                
                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return Ok(new { 
                    message = "Toutes les notifications ont été marquées comme lues",
                    count = response.Data 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage de toutes les notifications comme lues pour l'utilisateur {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Crée une notification personnalisée (réservé aux managers et admins)
        /// </summary>
        /// <param name="createDto">Données de création de la notification</param>
        /// <returns>Notification créée</returns>
        [HttpPost]
        [Authorize(Roles = $"{RoleTypes.Manager},{RoleTypes.Admin}")]
        public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationDto createDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var response = await _notificationService.CreateCustomNotificationAsync(createDto, currentUserId);
                
                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return CreatedAtAction(nameof(GetNotifications), new { id = response.Data.Id }, response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création d'une notification personnalisée par l'utilisateur {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Récupère le nombre de notifications non lues pour l'utilisateur connecté
        /// </summary>
        /// <returns>Statistiques des notifications non lues</returns>
        [HttpGet("unread-count")]
        public async Task<ActionResult<UnreadNotificationsResponse>> GetUnreadCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var response = await _notificationService.GetUnreadCountAsync(userId);
                
                if (!response.Success)
                {
                    return BadRequest(new { message = response.Message });
                }

                return Ok(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du compte des notifications non lues pour l'utilisateur {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "Erreur interne du serveur" });
            }
        }

        /// <summary>
        /// Extrait l'identifiant de l'utilisateur connecté à partir du token JWT
        /// </summary>
        /// <returns>Identifiant de l'utilisateur</returns>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("uid") ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Identifiant utilisateur invalide dans le token");
            }
            return userId;
        }
    }
}