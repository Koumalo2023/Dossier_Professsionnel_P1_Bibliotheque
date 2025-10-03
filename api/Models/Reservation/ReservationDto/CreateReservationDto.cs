using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// DTO pour créer une nouvelle réservation.
    /// </summary>
    public class CreateReservationDto
    {
        /// <summary>
        /// Identifiant du livre à réserver.
        /// </summary>
        [Required(ErrorMessage = "L'identifiant du livre est obligatoire.")]
        public Guid BookId { get; set; }
    }
}