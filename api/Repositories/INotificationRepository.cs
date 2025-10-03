using api.Helpers;
using api.Models;

namespace api.Repositories
{
    /// <summary>
    /// Interface définissant les opérations de gestion des notifications
    /// </summary>
    public interface INotificationRepository
    {
        /// <summary>
        /// Récupère les notifications d'un utilisateur avec filtrage optionnel
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <param name="read">Filtrer par statut de lecture (null pour tous)</param>
        /// <param name="type">Filtrer par type de notification</param>
        /// <param name="pageNumber">Numéro de page pour la pagination</param>
        /// <param name="pageSize">Taille de la page pour la pagination</param>
        /// <returns>Résultat paginé des notifications</returns>
        Task<PagedResult<Notification>> GetUserNotificationsAsync(Guid userId, bool? read = null, string? type = null, int pageNumber = 1, int pageSize = 20);

        /// <summary>
        /// Récupère une notification par son identifiant
        /// </summary>
        /// <param name="id">Identifiant de la notification</param>
        /// <returns>La notification ou null si non trouvée</returns>
        Task<Notification?> GetByIdAsync(Guid id);

        /// <summary>
        /// Crée une nouvelle notification
        /// </summary>
        /// <param name="notification">Notification à créer</param>
        /// <returns>La notification créée</returns>
        Task<Notification> CreateAsync(Notification notification);

        /// <summary>
        /// Marque une notification comme lue
        /// </summary>
        /// <param name="id">Identifiant de la notification</param>
        /// <returns>True si mise à jour réussie, false sinon</returns>
        Task<bool> MarkAsReadAsync(Guid id);

        /// <summary>
        /// Marque toutes les notifications d'un utilisateur comme lues
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Nombre de notifications mises à jour</returns>
        Task<int> MarkAllAsReadAsync(Guid userId);

        /// <summary>
        /// Récupère le nombre de notifications non lues pour un utilisateur
        /// </summary>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>Statistiques des notifications non lues</returns>
        Task<UserNotificationStats> GetUnreadCountAsync(Guid userId);

        /// <summary>
        /// Vérifie si une notification appartient à un utilisateur
        /// </summary>
        /// <param name="notificationId">Identifiant de la notification</param>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>True si la notification appartient à l'utilisateur</returns>
        Task<bool> BelongsToUserAsync(Guid notificationId, Guid userId);
    }

    /// <summary>
    /// Statistiques des notifications d'un utilisateur
    /// </summary>
    public class UserNotificationStats
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