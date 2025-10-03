using api;
using api.Data;
using api.Helpers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories
{
    /// <summary>
    /// Implémentation du repository pour la gestion des notifications
    /// </summary>
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initialise une nouvelle instance du repository des notifications
        /// </summary>
        /// <param name="context">Contexte de base de données</param>
        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Récupère les notifications d'un utilisateur avec filtrage optionnel
        /// </summary>
        public async Task<PagedResult<Notification>> GetUserNotificationsAsync(Guid userId, bool? read = null, string? type = null, int pageNumber = 1, int pageSize = 20)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .AsQueryable();

            // Appliquer les filtres
            if (read.HasValue)
            {
                query = query.Where(n => n.Read == read.Value);
            }

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(n => n.Type == type);
            }

            return PaginationHelper.CreatePagedResult(query, pageNumber, pageSize);
        }

        /// <summary>
        /// Récupère une notification par son identifiant
        /// </summary>
        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// Crée une nouvelle notification
        /// </summary>
        public async Task<Notification> CreateAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        /// <summary>
        /// Marque une notification comme lue
        /// </summary>
        public async Task<bool> MarkAsReadAsync(Guid id)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
                return false;

            notification.Read = true;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Marque toutes les notifications d'un utilisateur comme lues
        /// </summary>
        public async Task<int> MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.Read)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.Read = true;
            }

            await _context.SaveChangesAsync();
            return notifications.Count;
        }

        /// <summary>
        /// Récupère le nombre de notifications non lues pour un utilisateur
        /// </summary>
        public async Task<UserNotificationStats> GetUnreadCountAsync(Guid userId)
        {
            var stats = await _context.Notifications
                .Where(n => n.UserId == userId && !n.Read)
                .GroupBy(n => 1)
                .Select(g => new UserNotificationStats
                {
                    Count = g.Count(),
                    HighPriority = g.Count(n => n.Type == "HIGH")
                })
                .FirstOrDefaultAsync();

            return stats ?? new UserNotificationStats { Count = 0, HighPriority = 0 };
        }

        /// <summary>
        /// Vérifie si une notification appartient à un utilisateur
        /// </summary>
        public async Task<bool> BelongsToUserAsync(Guid notificationId, Guid userId)
        {
            return await _context.Notifications
                .AnyAsync(n => n.Id == notificationId && n.UserId == userId);
        }
    }
}