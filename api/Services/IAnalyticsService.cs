using api.Helpers;
using api.Models; 

namespace api.Services
{
    /// <summary>
    /// Interface pour le service de gestion des statistiques et analytics
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Récupère les catégories les plus populaires
        /// </summary>
        Task<ServiceResponse<IEnumerable<PopularCategoryDto>>> GetPopularCategoriesAsync(string period = "MONTH", int limit = 5);

        /// <summary>
        /// Récupère les tendances des emprunts par période
        /// </summary>
        Task<ServiceResponse<BorrowTrendsResponseDto>> GetBorrowTrendsAsync(string period = "MONTH", int months = 6);

        /// <summary>
        /// Récupère les statistiques des réservations
        /// </summary>
        Task<ServiceResponse<ReservationStatsDto>> GetReservationStatsAsync();

        /// <summary>
        /// Récupère les livres les plus empruntés
        /// </summary>
        Task<ServiceResponse<IEnumerable<TopBookDto>>> GetTopBooksAsync(string period = "MONTH", int limit = 10);

        /// <summary>
        /// Récupère les utilisateurs les plus actifs
        /// </summary>
        Task<ServiceResponse<IEnumerable<ActiveUserDto>>> GetActiveUsersAsync(int limit = 10);

        /// <summary>
        /// Récupère la vue d'ensemble des statistiques
        /// </summary>
        Task<ServiceResponse<AnalyticsOverviewDto>> GetAnalyticsOverviewAsync();

        /// <summary>
        /// Récupère les logs d'audit avec filtres
        /// </summary>
        Task<ServiceResponse<IEnumerable<AnalyticsAuditLogDto>>> GetAuditLogsAsync(Guid? userId = null, string? action = null, DateTime? startDate = null);
    }
}