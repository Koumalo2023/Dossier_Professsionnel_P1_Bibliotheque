using api.Helpers;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        /// <summary>
        /// Récupère tous les emprunts avec filtres
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLoans(
            [FromQuery] string? status = null,
            [FromQuery] Guid? userId = null,
            [FromQuery] bool? overdueOnly = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var currentUserId = GetCurrentUserId();
            var isAdminOrManager = User.IsInRole("Manager") || User.IsInRole("Admin");

            // Si l'utilisateur n'est pas admin/manager, il ne peut voir que ses propres emprunts
            if (!isAdminOrManager)
            {
                userId = currentUserId;
            }

            var result = await _loanService.GetAllLoansAsync(status, userId, overdueOnly, page, pageSize);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new
            {
                items = result.Data.Items,
                totalCount = result.Data.TotalCount,
                currentPage = result.Data.CurrentPage,
                pageSize = result.Data.PageSize,
                totalPages = result.Data.TotalPages
            });
        }

        /// <summary>
        /// Récupère les emprunts en retard (Manager, Admin uniquement)
        /// </summary>
        [HttpGet("overdue")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetOverdueLoans()
        {
            var result = await _loanService.GetOverdueLoansAsync();
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère l'historique des emprunts d'un utilisateur spécifique (Manager, Admin uniquement)
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetUserLoanHistory(
            Guid userId,
            [FromQuery] string? status = null,
            [FromQuery] int limit = 20)
        {
            var result = await _loanService.GetUserLoanHistoryAsync(userId, status, limit);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Crée un nouvel emprunt pour l'utilisateur connecté
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "User,Manager,Admin")]
        public async Task<IActionResult> CreateLoan([FromBody] CreateLoanDto createLoanDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "Utilisateur non authentifié" });

            var result = await _loanService.CreateLoanAsync(userId, createLoanDto);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors, message = result.Message });

            return CreatedAtAction(nameof(GetLoanById), new { id = result.Data.Id }, result.Data);
        }

        /// <summary>
        /// Récupère un emprunt par son ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetLoanById(Guid id)
        {
            var result = await _loanService.GetLoanByIdAsync(id);
            
            if (!result.Success)
                return NotFound(new { message = result.Message });

            var currentUserId = GetCurrentUserId();
            var isAdminOrManager = User.IsInRole("Manager") || User.IsInRole("Admin");

            // Vérifier que l'utilisateur peut voir cet emprunt
            if (!isAdminOrManager && result.Data.UserId != currentUserId)
                return Forbid();

            return Ok(result.Data);
        }

        /// <summary>
        /// Retourne un livre emprunté
        /// </summary>
        [HttpPut("{id}/return")]
        [Authorize]
        public async Task<IActionResult> ReturnLoan(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdminOrManager = User.IsInRole("Manager") || User.IsInRole("Admin");

            var result = await _loanService.ReturnLoanAsync(id, currentUserId, isAdminOrManager);
            
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new 
            { 
                message = result.Message,
                loan = result.Data 
            });
        }

        /// <summary>
        /// Prolonge un emprunt
        /// </summary>
        [HttpPut("{id}/extend")]
        [Authorize(Roles = "User,Manager,Admin")]
        public async Task<IActionResult> ExtendLoan(
            Guid id,
            [FromBody] ExtendLoanRequest request)
        {
            var currentUserId = GetCurrentUserId();

            var result = await _loanService.ExtendLoanAsync(id, currentUserId, request.AdditionalDays);
            
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new 
            { 
                message = result.Message,
                loan = result.Data 
            });
        }

        /// <summary>
        /// Envoie une notification de retard pour un emprunt (Manager, Admin uniquement)
        /// </summary>
        [HttpPut("{id}/notify-overdue")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> SendOverdueNotification(Guid id)
        {
            var result = await _loanService.SendOverdueNotificationAsync(id);
            
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Vérifie si un livre peut être emprunté
        /// </summary>
        [HttpGet("can-borrow/{bookId}")]
        [Authorize(Roles = "User,Manager,Admin")]
        public async Task<IActionResult> CanBorrowBook(Guid bookId)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "Utilisateur non authentifié" });

            var result = await _loanService.CanBorrowBookAsync(bookId, userId);
            
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère les statistiques d'emprunt de l'utilisateur connecté
        /// </summary>
        [HttpGet("stats")]
        [Authorize(Roles = "User,Manager,Admin")]
        public async Task<IActionResult> GetUserLoanStats()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "Utilisateur non authentifié" });

            var result = await _loanService.GetUserLoanStatsAsync(userId);
            
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère l'ID de l'utilisateur connecté
        /// </summary>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                return userId;
            }
            return Guid.Empty;
        }
    }

    /// <summary>
    /// Requête pour prolonger un emprunt
    /// </summary>
    public class ExtendLoanRequest
    {
        public int AdditionalDays { get; set; } = 7;
    }
}