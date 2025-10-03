using api.Helpers;
using api.Models;

namespace api.Services
{
    /// <summary>
    /// Interface pour le service de gestion des réservations
    /// </summary>
    public interface IReservationService
    {
        /// <summary>
        /// Crée une nouvelle réservation pour un utilisateur
        /// </summary>
        Task<ServiceResponse<ReservationDto>> CreateReservationAsync(Guid userId, CreateReservationDto createReservationDto);

        /// <summary>
        /// Récupère toutes les réservations avec pagination et filtres
        /// </summary>
        Task<ServiceResponse<PagedResult<ReservationDto>>> GetAllReservationsAsync(string? status = null, Guid? userId = null, Guid? bookId = null, int page = 1, int pageSize = 20);

        /// <summary>
        /// Récupère les réservations d'un utilisateur spécifique
        /// </summary>
        Task<ServiceResponse<PagedResult<ReservationDto>>> GetUserReservationsAsync(Guid userId, string? status = null, int page = 1, int pageSize = 20);

        /// <summary>
        /// Récupère une réservation par son ID
        /// </summary>
        Task<ServiceResponse<ReservationDto>> GetReservationByIdAsync(Guid id);

        /// <summary>
        /// Annule une réservation
        /// </summary>
        Task<ServiceResponse<bool>> CancelReservationAsync(Guid reservationId, Guid currentUserId, bool isAdminOrManager = false);

        /// <summary>
        /// Vérifie si un livre peut être réservé (non disponible)
        /// </summary>
        Task<ServiceResponse<bool>> CanReserveBookAsync(Guid bookId);

        /// <summary>
        /// Récupère les réservations expirées
        /// </summary>
        Task<ServiceResponse<IEnumerable<ReservationDto>>> GetExpiredReservationsAsync();

        /// <summary>
        /// Récupère les réservations disponibles
        /// </summary>
        Task<ServiceResponse<IEnumerable<ReservationDto>>> GetAvailableReservationsAsync();

        /// <summary>
        /// Met à jour le statut d'une réservation
        /// </summary>
        Task<ServiceResponse<ReservationDto>> UpdateReservationStatusAsync(Guid reservationId, string status, DateTime? availableSince = null);
    }
}