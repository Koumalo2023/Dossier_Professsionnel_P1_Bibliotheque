using api.Models;
using api.Helpers;

namespace api.Services
{
    /// <summary>
    /// Interface définissant les opérations de service pour la gestion des notifications
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Récupère les notifications d'un utilisateur avec filtrage optionnel
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="read">Filtrer par statut de lecture</param>
        /// <param name="type">Filtrer par type de notification</param>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de la page</param>
        /// <returns>Résultat paginé des notifications</returns>
        Task<ServiceResponse<PagedResult<NotificationDto>>> GetUserNotificationsAsync(Guid userId, bool? read = null, string? type = null, int pageNumber = 1, int pageSize = 20);

        /// <summary>
        /// Marque une notification comme lue
        /// </summary>
        /// <param name="notificationId">Identifiant de la notification</param>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Réponse indiquant le succès de l'opération</returns>
        Task<ServiceResponse<bool>> MarkAsReadAsync(Guid notificationId, Guid userId);

        /// <summary>
        /// Marque toutes les notifications d'un utilisateur comme lues
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Réponse indiquant le nombre de notifications mises à jour</returns>
        Task<ServiceResponse<int>> MarkAllAsReadAsync(Guid userId);

        /// <summary>
        /// Crée une notification personnalisée (réservé aux managers et admins)
        /// </summary>
        /// <param name="createDto">Données de création de la notification</param>
        /// <param name="currentUserId">Identifiant de l'utilisateur créant la notification</param>
        /// <returns>Réponse contenant la notification créée</returns>
        Task<ServiceResponse<NotificationDto>> CreateCustomNotificationAsync(CreateNotificationDto createDto, Guid currentUserId);

        /// <summary>
        /// Récupère le nombre de notifications non lues pour un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Réponse contenant les statistiques des notifications non lues</returns>
        Task<ServiceResponse<UnreadNotificationsResponse>> GetUnreadCountAsync(Guid userId);

        /// <summary>
        /// Vérifie si l'utilisateur a la permission de gérer les notifications
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>True si l'utilisateur a les permissions nécessaires</returns>
        Task<bool> CanManageNotificationsAsync(Guid userId);
    }

    /// <summary>
    /// Réponse pour le nombre de notifications non lues
    /// </summary>
    public class UnreadNotificationsResponse
    {
        /// <summary>
        /// Nombre total de notifications non lues
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Nombre de notifications haute priorité non lues
        /// </summary>
        public int HighPriority { get; set; }
    }
}