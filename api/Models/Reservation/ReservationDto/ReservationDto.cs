using System;

namespace api.Models
{
    /// <summary>
    /// DTO pour représenter une réservation.
    /// </summary>
    public class ReservationDto
    {
        /// <summary>
        /// Identifiant unique de la réservation.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur ayant fait la réservation.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Nom de l'utilisateur ayant fait la réservation.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Identifiant du livre réservé.
        /// </summary>
        public Guid BookId { get; set; }

        /// <summary>
        /// Titre du livre réservé.
        /// </summary>
        public string BookTitle { get; set; } = null!;

        /// <summary>
        /// Date de création de la réservation.
        /// </summary>
        public DateTime ReservationDate { get; set; }

        /// <summary>
        /// Date limite pour récupérer le livre.
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Statut de la réservation.
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// Date à laquelle le livre est devenu disponible.
        /// </summary>
        public DateTime? AvailableSince { get; set; }

        /// <summary>
        /// Identifiant de l'emprunt créé suite à la réservation.
        /// </summary>
        public Guid? FulfilledLoanId { get; set; }
    }
}