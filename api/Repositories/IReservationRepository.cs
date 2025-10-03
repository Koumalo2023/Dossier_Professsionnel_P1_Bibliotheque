using api.Helpers;
using api.Models;

namespace api.Repositories
{
    /// <summary>
    /// Interface pour le repository des réservations
    /// </summary>
    public interface IReservationRepository
    {
        /// <summary>
        /// Récupère toutes les réservations avec pagination et filtres
        /// </summary>
        Task<PagedResult<Reservation>> GetAllAsync(string? status = null, Guid? userId = null, Guid? bookId = null, int page = 1, int pageSize = 20);

        /// <summary>
        /// Récupère une réservation par son ID
        /// </summary>
        Task<Reservation?> GetByIdAsync(Guid id);

        /// <summary>
        /// Récupère les réservations d'un utilisateur spécifique
        /// </summary>
        Task<PagedResult<Reservation>> GetByUserIdAsync(Guid userId, string? status = null, int page = 1, int pageSize = 20);

        /// <summary>
        /// Vérifie si un utilisateur a déjà une réservation en attente pour un livre
        /// </summary>
        Task<bool> HasPendingReservationAsync(Guid userId, Guid bookId);

        /// <summary>
        /// Vérifie si un livre est disponible (pas d'emprunts actifs et copies disponibles)
        /// </summary>
        Task<bool> IsBookAvailableAsync(Guid bookId);

        /// <summary>
        /// Ajoute une nouvelle réservation
        /// </summary>
        Task<Reservation> AddAsync(Reservation reservation);

        /// <summary>
        /// Met à jour une réservation existante
        /// </summary>
        Task<Reservation> UpdateAsync(Reservation reservation);

        /// <summary>
        /// Supprime une réservation
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Vérifie si une réservation peut être annulée (statut != FULFILLED ou EXPIRED)
        /// </summary>
        Task<bool> CanCancelAsync(Guid reservationId);

        /// <summary>
        /// Récupère les réservations expirées
        /// </summary>
        Task<IEnumerable<Reservation>> GetExpiredReservationsAsync();

        /// <summary>
        /// Récupère les réservations disponibles (statut AVAILABLE)
        /// </summary>
        Task<IEnumerable<Reservation>> GetAvailableReservationsAsync();

        /// <summary>
        /// Met à jour le statut d'une réservation
        /// </summary>
        Task<bool> UpdateStatusAsync(Guid reservationId, string status, DateTime? availableSince = null);
    }
}