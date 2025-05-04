using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;

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
        public string Roles { get; set; } = "User";

        /// <summary>
        /// Date de création de l'utilisateur.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de mise à jour de l'utilisateur.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Méthode helper pour les rôles
        public List<string> GetRolesList() =>
            Roles.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        public void SetRolesList(IEnumerable<string> roles) =>
            Roles = string.Join(",", roles.Distinct());

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
