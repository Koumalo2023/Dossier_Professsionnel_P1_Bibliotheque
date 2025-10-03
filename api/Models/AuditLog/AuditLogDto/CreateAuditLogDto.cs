using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// DTO pour créer un nouvel enregistrement d'audit.
    /// </summary>
    public class CreateAuditLogDto
    {
        /// <summary>
        /// Identifiant de l'utilisateur qui a effectué l'action.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Nom de l'utilisateur.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Action effectuée.
        /// </summary>
        [Required(ErrorMessage = "L'action est obligatoire.")]
        public string Action { get; set; } = null!;

        /// <summary>
        /// Nom de l'entité concernée.
        /// </summary>
        public string? EntityName { get; set; }

        /// <summary>
        /// Identifiant de l'entité concernée.
        /// </summary>
        public Guid? EntityId { get; set; }

        /// <summary>
        /// Détails de l'action.
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Adresse IP de l'utilisateur.
        /// </summary>
        public string? IpAddress { get; set; }
    }
}