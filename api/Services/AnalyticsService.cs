using api.Helpers;
using api.Models;
using api.Repositories;

namespace api.Services
{
    /// <summary>
    /// Service pour la gestion des statistiques et analytics
    /// </summary>
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        public AnalyticsService(IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository;
        }

        public async Task<ServiceResponse<IEnumerable<PopularCategoryDto>>> GetPopularCategoriesAsync(string period = "MONTH", int limit = 5)
        {
            try
            {
                if (limit <= 0 || limit > 100)
                {
                    return new ServiceResponse<IEnumerable<PopularCategoryDto>>
                    {
                        Success = false,
                        Message = "La limite doit être comprise entre 1 et 100"
                    };
                }

                var validPeriods = new[] { "WEEK", "MONTH", "QUARTER", "YEAR", "ALL_TIME" };
                if (!validPeriods.Contains(period.ToUpper()))
                {
                    return new ServiceResponse<IEnumerable<PopularCategoryDto>>
                    {
                        Success = false,
                        Message = "Période invalide. Valeurs acceptées : WEEK, MONTH, QUARTER, YEAR, ALL_TIME"
                    };
                }

                var categories = await _analyticsRepository.GetPopularCategoriesAsync(period, limit);

                return new ServiceResponse<IEnumerable<PopularCategoryDto>>
                {
                    Success = true,
                    Data = categories
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<PopularCategoryDto>>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des catégories populaires",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<BorrowTrendsResponseDto>> GetBorrowTrendsAsync(string period = "MONTH", int months = 6)
        {
            try
            {
                if (months <= 0 || months > 24)
                {
                    return new ServiceResponse<BorrowTrendsResponseDto>
                    {
                        Success = false,
                        Message = "Le nombre de mois doit être compris entre 1 et 24"
                    };
                }

                var validPeriods = new[] { "WEEK", "MONTH" };
                if (!validPeriods.Contains(period.ToUpper()))
                {
                    return new ServiceResponse<BorrowTrendsResponseDto>
                    {
                        Success = false,
                        Message = "Période invalide. Valeurs acceptées : WEEK, MONTH"
                    };
                }

                var trends = await _analyticsRepository.GetBorrowTrendsAsync(period, months);

                return new ServiceResponse<BorrowTrendsResponseDto>
                {
                    Success = true,
                    Data = trends
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<BorrowTrendsResponseDto>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des tendances d'emprunts",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<ReservationStatsDto>> GetReservationStatsAsync()
        {
            try
            {
                var stats = await _analyticsRepository.GetReservationStatsAsync();

                return new ServiceResponse<ReservationStatsDto>
                {
                    Success = true,
                    Data = stats
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ReservationStatsDto>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des statistiques des réservations",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<TopBookDto>>> GetTopBooksAsync(string period = "MONTH", int limit = 10)
        {
            try
            {
                if (limit <= 0 || limit > 50)
                {
                    return new ServiceResponse<IEnumerable<TopBookDto>>
                    {
                        Success = false,
                        Message = "La limite doit être comprise entre 1 et 50"
                    };
                }

                var validPeriods = new[] { "WEEK", "MONTH", "QUARTER", "YEAR", "ALL_TIME" };
                if (!validPeriods.Contains(period.ToUpper()))
                {
                    return new ServiceResponse<IEnumerable<TopBookDto>>
                    {
                        Success = false,
                        Message = "Période invalide. Valeurs acceptées : WEEK, MONTH, QUARTER, YEAR, ALL_TIME"
                    };
                }

                var topBooks = await _analyticsRepository.GetTopBooksAsync(period, limit);

                return new ServiceResponse<IEnumerable<TopBookDto>>
                {
                    Success = true,
                    Data = topBooks
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<TopBookDto>>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des livres les plus empruntés",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<ActiveUserDto>>> GetActiveUsersAsync(int limit = 10)
        {
            try
            {
                if (limit <= 0 || limit > 50)
                {
                    return new ServiceResponse<IEnumerable<ActiveUserDto>>
                    {
                        Success = false,
                        Message = "La limite doit être comprise entre 1 et 50"
                    };
                }

                var activeUsers = await _analyticsRepository.GetActiveUsersAsync(limit);

                return new ServiceResponse<IEnumerable<ActiveUserDto>>
                {
                    Success = true,
                    Data = activeUsers
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<ActiveUserDto>>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des utilisateurs actifs",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<AnalyticsOverviewDto>> GetAnalyticsOverviewAsync()
        {
            try
            {
                var overview = await _analyticsRepository.GetAnalyticsOverviewAsync();

                return new ServiceResponse<AnalyticsOverviewDto>
                {
                    Success = true,
                    Data = overview
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<AnalyticsOverviewDto>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération de la vue d'ensemble",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ServiceResponse<IEnumerable<AnalyticsAuditLogDto>>> GetAuditLogsAsync(Guid? userId = null, string? action = null, DateTime? startDate = null)
        {
            try
            {
                var auditLogs = await _analyticsRepository.GetAuditLogsAsync(userId, action, startDate);
                
                return new ServiceResponse<IEnumerable<AnalyticsAuditLogDto>>
                {
                    Success = true,
                    Data = auditLogs
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<AnalyticsAuditLogDto>>
                {
                    Success = false,
                    Message = "Erreur lors de la récupération des logs d'audit",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
