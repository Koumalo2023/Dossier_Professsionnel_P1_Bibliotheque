
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    /// <summary>
    /// Représente un enregistrement d'audit pour tracer les actions des utilisateurs.
    /// </summary>
    public class AuditLog
    {
        /// <summary>
        /// Identifiant unique de l'enregistrement d'audit (généré automatiquement).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur qui a effectué l'action (optionnel).
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Nom de l'utilisateur (conservé même si l'utilisateur est supprimé).
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Action effectuée (ex: "BOOK_DELETED", "USER_ROLE_CHANGED").
        /// </summary>
        [Required]
        public string Action { get; set; } = null!;

        /// <summary>
        /// Nom de l'entité concernée (ex: "Book", "User").
        /// </summary>
        public string? EntityName { get; set; }

        /// <summary>
        /// Identifiant de l'entité concernée.
        /// </summary>
        public Guid? EntityId { get; set; }

        /// <summary>
        /// Détails de l'action (JSON des anciennes/nouvelles valeurs).
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Date de création de l'enregistrement d'audit.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Adresse IP de l'utilisateur qui a effectué l'action.
        /// </summary>
        public string? IpAddress { get; set; }
    }
}