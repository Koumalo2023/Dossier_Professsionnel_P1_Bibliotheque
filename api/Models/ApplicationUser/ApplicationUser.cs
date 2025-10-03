using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations; 

namespace api.Models
{
    /// <summary>
    /// Représente un utilisateur de l'application.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Nom complet de l'utilisateur.
        /// </summary>
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Date de création de l'utilisateur.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de mise à jour de l'utilisateur.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Rôles de l'utilisateur.
        /// </summary>
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

        /// <summary>
        /// Emprunts effectués par l'utilisateur.
        /// </summary>
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();

        /// <summary>
        /// Réservations effectuées par l'utilisateur.
        /// </summary>
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        /// <summary>
        /// Notifications reçues par l'utilisateur.
        /// </summary>
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

}
