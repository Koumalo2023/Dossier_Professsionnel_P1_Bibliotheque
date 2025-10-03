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
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Crée une nouvelle réservation pour l'utilisateur connecté
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "User,Manager,Admin")]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto createReservationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "Utilisateur non authentifié" });

            var result = await _reservationService.CreateReservationAsync(userId, createReservationDto);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors, message = result.Message });

            return CreatedAtAction(nameof(GetReservationById), new { id = result.Data.Id }, result.Data);
        }

        /// <summary>
        /// Récupère les réservations avec filtres
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReservations(
            [FromQuery] string? status = null,
            [FromQuery] Guid? userId = null,
            [FromQuery] Guid? bookId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var currentUserId = GetCurrentUserId();
            var isAdminOrManager = User.IsInRole("Manager") || User.IsInRole("Admin");

            // Si l'utilisateur n'est pas admin/manager, il ne peut voir que ses propres réservations
            if (!isAdminOrManager)
            {
                userId = currentUserId;
            }

            var result = await _reservationService.GetAllReservationsAsync(status, userId, bookId, page, pageSize);
            
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
        /// Récupère une réservation par son ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetReservationById(Guid id)
        {
            var result = await _reservationService.GetReservationByIdAsync(id);
            
            if (!result.Success)
                return NotFound(new { message = result.Message });

            var currentUserId = GetCurrentUserId();
            var isAdminOrManager = User.IsInRole("Manager") || User.IsInRole("Admin");

            // Vérifier que l'utilisateur peut voir cette réservation
            if (!isAdminOrManager && result.Data.UserId != currentUserId)
                return Forbid();

            return Ok(result.Data);
        }

        /// <summary>
        /// Annule une réservation
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> CancelReservation(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            var isAdminOrManager = User.IsInRole("Manager") || User.IsInRole("Admin");

            var result = await _reservationService.CancelReservationAsync(id, currentUserId, isAdminOrManager);
            
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        /// <summary>
        /// Vérifie si un livre peut être réservé
        /// </summary>
        [HttpGet("can-reserve/{bookId}")]
        [Authorize(Roles = "User,Manager,Admin")]
        public async Task<IActionResult> CanReserveBook(Guid bookId)
        {
            var result = await _reservationService.CanReserveBookAsync(bookId);
            
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new 
            { 
                canReserve = result.Data,
                message = result.Message
            });
        }

        /// <summary>
        /// Récupère les réservations expirées (Manager, Admin uniquement)
        /// </summary>
        [HttpGet("expired")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetExpiredReservations()
        {
            var result = await _reservationService.GetExpiredReservationsAsync();
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Récupère les réservations disponibles (Manager, Admin uniquement)
        /// </summary>
        [HttpGet("available")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetAvailableReservations()
        {
            var result = await _reservationService.GetAvailableReservationsAsync();
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(result.Data);
        }

        /// <summary>
        /// Met à jour le statut d'une réservation (Manager, Admin uniquement)
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateReservationStatus(Guid id, [FromBody] UpdateReservationStatusRequest request)
        {
            var result = await _reservationService.UpdateReservationStatusAsync(id, request.Status, request.AvailableSince);
            
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new
            {
                message = result.Message,
                reservation = result.Data
            });
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
    /// Requête pour mettre à jour le statut d'une réservation
    /// </summary>
    public class UpdateReservationStatusRequest
    {
        public string Status { get; set; } = null!;
        public DateTime? AvailableSince { get; set; }
    }
}