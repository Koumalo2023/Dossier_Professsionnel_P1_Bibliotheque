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
        public string Name { get; set; }

        /// <summary>
        /// Rôle de l'utilisateur (USER ou ADMIN).
        /// </summary>
        public string Role { get; set; } = "USER";

        /// <summary>
        /// Date de création de l'utilisateur.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de mise à jour de l'utilisateur.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Emprunts effectués par l'utilisateur.
        /// </summary>
        public ICollection<Loan> Loans { get; set; }

        /// <summary>
        /// Notifications reçues par l'utilisateur.
        /// </summary>
        public ICollection<Notification> Notifications { get; set; }
    }
}
