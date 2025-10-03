using api.Helpers;
using api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Repositories
{
    /// <summary>
    /// Interface pour le repository des statistiques et analytics
    /// </summary>
    public interface IAnalyticsRepository
    {
        /// <summary>
        /// Récupère les catégories les plus populaires
        /// </summary>
        Task<IEnumerable<PopularCategoryDto>> GetPopularCategoriesAsync(string period = "MONTH", int limit = 5);

        /// <summary>
        /// Récupère les tendances des emprunts par période
        /// </summary>
        Task<BorrowTrendsResponseDto> GetBorrowTrendsAsync(string period = "MONTH", int months = 6);

        /// <summary>
        /// Récupère les statistiques des réservations
        /// </summary>
        Task<ReservationStatsDto> GetReservationStatsAsync();

        /// <summary>
        /// Récupère les livres les plus empruntés
        /// </summary>
        Task<IEnumerable<TopBookDto>> GetTopBooksAsync(string period = "MONTH", int limit = 10);

        /// <summary>
        /// Récupère les utilisateurs les plus actifs
        /// </summary>
        Task<IEnumerable<ActiveUserDto>> GetActiveUsersAsync(int limit = 10);

        /// <summary>
        /// Récupère la vue d'ensemble des statistiques
        /// </summary>
        Task<AnalyticsOverviewDto> GetAnalyticsOverviewAsync();

        /// <summary>
        /// Récupère les logs d'audit avec filtres
        /// </summary>
        Task<IEnumerable<AnalyticsAuditLogDto>> GetAuditLogsAsync(Guid? userId = null, string? action = null, DateTime? startDate = null);
    }
}