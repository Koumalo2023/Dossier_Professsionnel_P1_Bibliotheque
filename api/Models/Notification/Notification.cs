using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// Représente une notification envoyée à un utilisateur.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Identifiant unique de la notification (généré automatiquement).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur associé à la notification.
        /// </summary>
        [Required(ErrorMessage = "L'utilisateur est obligatoire.")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Utilisateur qui a reçu la notification.
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// Message de la notification.
        /// </summary>
        [Required(ErrorMessage = "Le message est obligatoire.")]
        public string Message { get; set; }

        /// <summary>
        /// Type de notification (REMINDER, INFO, WARNING).
        /// </summary>
        [Required(ErrorMessage = "Le type est obligatoire.")]
        public string Type { get; set; }

        /// <summary>
        /// Indique si la notification a été lue.
        /// </summary>
        public bool Read { get; set; } = false;

        /// <summary>
        /// Date de création de la notification.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
