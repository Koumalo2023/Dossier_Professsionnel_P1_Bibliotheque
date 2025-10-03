using api.Data;
using api.Models; 
using Microsoft.EntityFrameworkCore; 

namespace api.Repositories
{
    /// <summary>
    /// Repository pour la gestion des statistiques et analytics
    /// </summary>
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AppDbContext _context;

        public AnalyticsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PopularCategoryDto>> GetPopularCategoriesAsync(string period = "MONTH", int limit = 5)
        {
            var cutoffDate = GetCutoffDate(period);

            var categoryStats = await _context.Loans
                .Where(l => l.LoanDate >= cutoffDate)
                .GroupBy(l => l.Book.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    BorrowCount = g.Count(),
                    TotalBorrows = _context.Loans.Count(l => l.LoanDate >= cutoffDate)
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(limit)
                .ToListAsync();

            var totalBorrows = categoryStats.Sum(x => x.BorrowCount);

            return categoryStats.Select(x => new PopularCategoryDto
            {
                CategoryId = x.Category.Id,
                Name = x.Category.Name,
                BorrowCount = x.BorrowCount,
                Percentage = totalBorrows > 0 ? Math.Round((x.BorrowCount * 100.0) / totalBorrows, 1) : 0
            });
        }

        public async Task<BorrowTrendsResponseDto> GetBorrowTrendsAsync(string period = "MONTH", int months = 6)
        {
            var endDate = DateTime.UtcNow;
            var startDate = period.ToUpper() switch
            {
                "MONTH" => endDate.AddMonths(-months),
                "WEEK" => endDate.AddDays(-(months * 7)),
                "QUARTER" => endDate.AddMonths(-(months * 3)),
                "YEAR" => endDate.AddYears(-months),
                _ => endDate.AddMonths(-months)
            };

            var trends = new List<BorrowTrendDto>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                DateTime periodStart, periodEnd;
                string periodLabel;

                if (period.ToUpper() == "MONTH")
                {
                    periodStart = new DateTime(currentDate.Year, currentDate.Month, 1);
                    periodEnd = periodStart.AddMonths(1).AddDays(-1);
                    periodLabel = $"{currentDate.Year}-{currentDate.Month:D2}";
                    currentDate = currentDate.AddMonths(1);
                }
                else // WEEK
                {
                    var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);
                    periodStart = startOfWeek;
                    periodEnd = startOfWeek.AddDays(6);
                    periodLabel = $"{currentDate.Year}-W{GetWeekOfYear(currentDate):D2}";
                    currentDate = currentDate.AddDays(7);
                }

                var borrowCount = await _context.Loans
                    .CountAsync(l => l.LoanDate >= periodStart && l.LoanDate <= periodEnd);

                trends.Add(new BorrowTrendDto
                {
                    Period = periodLabel,
                    BorrowCount = borrowCount,
                    Growth = 0 // Calculé après
                });
            }

            // Calculer la croissance
            for (int i = 1; i < trends.Count; i++)
            {
                var current = trends[i];
                var previous = trends[i - 1];
                
                if (previous.BorrowCount > 0)
                {
                    current.Growth = Math.Round(((current.BorrowCount - previous.BorrowCount) * 100.0) / previous.BorrowCount, 1);
                }
            }

            var averageGrowth = trends.Count > 1 ? trends.Skip(1).Average(t => t.Growth) : 0;

            return new BorrowTrendsResponseDto
            {
                Period = period,
                Data = trends,
                AverageGrowth = Math.Round(averageGrowth, 1)
            };
        }

        public async Task<ReservationStatsDto> GetReservationStatsAsync()
        {
            var totalReservations = await _context.Reservations.CountAsync();
            var pendingReservations = await _context.Reservations.CountAsync(r => r.Status == "PENDING");
            var fulfilledReservations = await _context.Reservations.CountAsync(r => r.Status == "FULFILLED");
            var expiredReservations = await _context.Reservations.CountAsync(r => r.Status == "EXPIRED");

            // Calcul du temps moyen de réalisation (en utilisant AvailableSince comme date de disponibilité)
            var fulfilledReservationsWithTime = await _context.Reservations
                .Where(r => r.Status == "FULFILLED" && r.AvailableSince.HasValue)
                .Select(r => new
                {
                    Duration = (r.AvailableSince.Value - r.ReservationDate).TotalHours
                })
                .ToListAsync();

            var averageFulfillmentTime = fulfilledReservationsWithTime.Any()
                ? fulfilledReservationsWithTime.Average(r => r.Duration)
                : 0;

            // Livre le plus réservé
            var mostReservedBook = await _context.Reservations
                .GroupBy(r => r.Book)
                .Select(g => new { BookTitle = g.Key.Title, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync();

            return new ReservationStatsDto
            {
                TotalReservations = totalReservations,
                PendingReservations = pendingReservations,
                FulfilledReservations = fulfilledReservations,
                ExpiredReservations = expiredReservations,
                AverageFulfillmentTimeHours = Math.Round(averageFulfillmentTime, 1),
                MostReservedBook = mostReservedBook?.BookTitle ?? "Aucun"
            };
        }

        public async Task<IEnumerable<TopBookDto>> GetTopBooksAsync(string period = "MONTH", int limit = 10)
        {
            var cutoffDate = GetCutoffDate(period);

            var topBooks = await _context.Loans
                .Where(l => l.LoanDate >= cutoffDate)
                .GroupBy(l => l.Book)
                .Select(g => new TopBookDto
                {
                    BookId = g.Key.Id,
                    Title = g.Key.Title,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(b => b.BorrowCount)
                .Take(limit)
                .ToListAsync();

            return topBooks;
        }

        public async Task<IEnumerable<ActiveUserDto>> GetActiveUsersAsync(int limit = 10)
        {
            var activeUsers = await _context.Loans
                .Where(l => l.LoanDate >= DateTime.UtcNow.AddMonths(-1)) // Derniers 30 jours
                .GroupBy(l => l.User)
                .Select(g => new ActiveUserDto
                {
                    UserId = g.Key.Id,
                    Name = g.Key.Name,
                    LoanCount = g.Count()
                })
                .OrderByDescending(u => u.LoanCount)
                .Take(limit)
                .ToListAsync();

            return activeUsers;
        }

        public async Task<AnalyticsOverviewDto> GetAnalyticsOverviewAsync()
        {
            var totalBooks = await _context.Books.CountAsync();
            var availableBooks = await _context.Books.CountAsync(b => b.AvailableCopies > 0);
            var activeLoans = await _context.Loans.CountAsync(l => l.Status == "BORROWED");
            var overdueLoans = await _context.Loans
                .CountAsync(l => l.Status == "BORROWED" && l.DueDate < DateTime.UtcNow);
            var pendingReservations = await _context.Reservations.CountAsync(r => r.Status == "PENDING");

            var activeUsersLast30Days = await _context.Loans
                .Where(l => l.LoanDate >= DateTime.UtcNow.AddDays(-30))
                .Select(l => l.UserId)
                .Distinct()
                .CountAsync();

            return new AnalyticsOverviewDto
            {
                TotalBooks = totalBooks,
                AvailableBooks = availableBooks,
                ActiveLoans = activeLoans,
                OverdueLoans = overdueLoans,
                PendingReservations = pendingReservations,
                ActiveUsersLast30Days = activeUsersLast30Days
            };
        }

        private DateTime GetCutoffDate(string period)
        {
            return period.ToUpper() switch
            {
                "WEEK" => DateTime.UtcNow.AddDays(-7),
                "MONTH" => DateTime.UtcNow.AddMonths(-1),
                "QUARTER" => DateTime.UtcNow.AddMonths(-3),
                "YEAR" => DateTime.UtcNow.AddYears(-1),
                "ALL_TIME" => DateTime.MinValue,
                _ => DateTime.UtcNow.AddMonths(-1)
            };
        }

        public async Task<IEnumerable<AnalyticsAuditLogDto>> GetAuditLogsAsync(Guid? userId = null, string? action = null, DateTime? startDate = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(a => a.UserId == userId.Value);
            }

            if (!string.IsNullOrEmpty(action))
            {
                query = query.Where(a => a.Action == action);
            }

            if (startDate.HasValue)
            {
                query = query.Where(a => a.CreatedAt >= startDate.Value);
            }

            var auditLogs = await query
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AnalyticsAuditLogDto
                {
                    Id = a.Id,
                    UserName = a.UserName,
                    Action = a.Action,
                    EntityName = a.EntityName,
                    EntityId = a.EntityId.HasValue ? a.EntityId.Value.ToString() : null,
                    Details = a.Details,
                    CreatedAt = a.CreatedAt,
                    IpAddress = a.IpAddress
                })
                .ToListAsync();

            return auditLogs;
        }

        private int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
        }
    }
}