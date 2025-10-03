using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// DTO pour mettre à jour une réservation.
    /// </summary>
    public class UpdateReservationDto
    {
        /// <summary>
        /// Nouveau statut de la réservation.
        /// </summary>
        [Required(ErrorMessage = "Le statut est obligatoire.")]
        public string Status { get; set; } = null!;

        /// <summary>
        /// Date limite pour récupérer le livre.
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

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