using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    /// <summary>
    /// Représente un rôle dans l'application.
    /// </summary>
    public class ApplicationRole : IdentityRole<Guid>
    {
        /// <summary>
        /// Constructeur par défaut requis par Entity Framework.
        /// </summary>
        public ApplicationRole() : base() { }

        /// <summary>
        /// Constructeur pour créer un nouveau rôle.
        /// </summary>
        /// <param name="roleName">Nom du rôle</param>
        public ApplicationRole(string roleName) : base(roleName) { }

        /// <summary>
        /// Description du rôle.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Utilisateurs associés à ce rôle.
        /// </summary>
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }
}