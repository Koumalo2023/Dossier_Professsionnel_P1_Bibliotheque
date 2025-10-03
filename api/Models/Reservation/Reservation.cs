using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    /// <summary>
    /// Représente une réservation de livre par un utilisateur.
    /// </summary>
    public class Reservation
    {
        /// <summary>
        /// Identifiant unique de la réservation.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur ayant fait la réservation.
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Utilisateur associé à la réservation.
        /// </summary>
        public ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Identifiant du livre réservé.
        /// </summary>
        [Required]
        public Guid BookId { get; set; }

        /// <summary>
        /// Livre associé à la réservation.
        /// </summary>
        public Book Book { get; set; } = null!;

        /// <summary>
        /// Date de création de la réservation.
        /// </summary>
        public DateTime ReservationDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date limite pour récupérer le livre après notification de disponibilité (ex: 48h).
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Statut de la réservation : PENDING, AVAILABLE, EXPIRED, CANCELLED, FULFILLED.
        /// </summary>
        [Required]
        public string Status { get; set; } = "PENDING";

        /// <summary>
        /// Date à laquelle le livre est devenu disponible pour cet utilisateur.
        /// </summary>
        public DateTime? AvailableSince { get; set; }

        /// <summary>
        /// Optionnel : Référence à l'emprunt créé suite à la réservation (si applicable).
        /// </summary>
        public Guid? FulfilledLoanId { get; set; }

        /// <summary>
        /// Emprunt créé suite à la réservation (si applicable).
        /// </summary>
        public Loan? FulfilledLoan { get; set; }
    }
}