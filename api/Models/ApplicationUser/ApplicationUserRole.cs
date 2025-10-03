using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    /// <summary>
    /// Représente la relation entre un utilisateur et un rôle.
    /// </summary>
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        /// <summary>
        /// Utilisateur associé au rôle.
        /// </summary>
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// Rôle associé à l'utilisateur.
        /// </summary>
        public virtual ApplicationRole Role { get; set; }
    }
}