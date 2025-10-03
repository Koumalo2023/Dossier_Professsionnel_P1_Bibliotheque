using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// Représente un emprunt d'un livre par un utilisateur.
    /// </summary>
    public class Loan
    {
        /// <summary>
        /// Identifiant unique de l'emprunt (généré automatiquement).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur qui a emprunté le livre.
        /// </summary>
        [Required(ErrorMessage = "L'utilisateur est obligatoire.")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Utilisateur associé à l'emprunt.
        /// </summary>
        public ApplicationUser User { get; set; } = null!; // Fully qualified to avoid namespace conflict

        /// <summary>
        /// Identifiant du livre emprunté.
        /// </summary>
        [Required(ErrorMessage = "Le livre est obligatoire.")]
        public Guid BookId { get; set; }

        /// <summary>
        /// Livre associé à l'emprunt.
        /// </summary>
        public Book Book { get; set; } = null!;

        /// <summary>
        /// Date de l'emprunt.
        /// </summary>
        public DateTime LoanDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date limite de retour du livre.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Date effective de retour du livre (optionnelle).
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Statut de l'emprunt (BORROWED, RETURNED, LATE).
        /// </summary>
        [Required(ErrorMessage = "Le statut est obligatoire.")]
        public string Status { get; set; } = "BORROWED";
    }

}
