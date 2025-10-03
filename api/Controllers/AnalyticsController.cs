using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// Récupère les catégories les plus populaires
        /// </summary>
        [HttpGet("categories/popular")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetPopularCategories(
            [FromQuery] string period = "MONTH",
            [FromQuery] int limit = 5)
        {
            var result = await _analyticsService.GetPopularCategoriesAsync(period, limit);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors, message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère les tendances des emprunts par période
        /// </summary>
        [HttpGet("trends/borrows")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetBorrowTrends(
            [FromQuery] string period = "MONTH",
            [FromQuery] int months = 6)
        {
            var result = await _analyticsService.GetBorrowTrendsAsync(period, months);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors, message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère les statistiques des réservations
        /// </summary>
        [HttpGet("reservations/stats")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetReservationStats()
        {
            var result = await _analyticsService.GetReservationStatsAsync();
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors, message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère les livres les plus empruntés
        /// </summary>
        [HttpGet("books/top")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetTopBooks(
            [FromQuery] string period = "MONTH",
            [FromQuery] int limit = 10)
        {
            var result = await _analyticsService.GetTopBooksAsync(period, limit);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors, message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère les utilisateurs les plus actifs
        /// </summary>
        [HttpGet("users/active")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetActiveUsers(
            [FromQuery] int limit = 10)
        {
            var result = await _analyticsService.GetActiveUsersAsync(limit);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors, message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère la vue d'ensemble des statistiques
        /// </summary>
        [HttpGet("overview")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetAnalyticsOverview()
        {
            var result = await _analyticsService.GetAnalyticsOverviewAsync();
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors, message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère les logs d'audit (Admin uniquement)
        /// </summary>
        [HttpGet("audit/logs")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] Guid? userId = null,
            [FromQuery] string? action = null,
            [FromQuery] DateTime? startDate = null)
        {
            var result = await _analyticsService.GetAuditLogsAsync(userId, action, startDate);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors, message = result.Message });

            return Ok(result.Data);
        }
    }
}