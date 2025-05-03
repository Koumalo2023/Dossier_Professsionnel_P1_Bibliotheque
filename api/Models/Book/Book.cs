using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// Représente un livre dans la bibliothèque.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Identifiant unique du livre (généré automatiquement).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Titre du livre.
        /// </summary>
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        public string Title { get; set; }

        /// <summary>
        /// Auteur du livre.
        /// </summary>
        [Required(ErrorMessage = "L'auteur est obligatoire.")]
        public string Author { get; set; }

        /// <summary>
        /// Catégorie du livre.
        /// </summary>
        [Required(ErrorMessage = "La catégorie est obligatoire.")]
        public string Category { get; set; }

        /// <summary>
        /// ISBN unique du livre.
        /// </summary>
        [Required(ErrorMessage = "L'ISBN est obligatoire.")]
        public string Isbn { get; set; }

        /// <summary>
        /// Date de publication du livre (optionnelle).
        /// </summary>
        public DateTime? PublicationDate { get; set; }

        /// <summary>
        /// URL de l'image de couverture (optionnelle).
        /// </summary>
        public string CoverUrl { get; set; }

        /// <summary>
        /// Nombre de copies disponibles.
        /// </summary>
        public int AvailableCopies { get; set; } = 1;

        /// <summary>
        /// Nombre total de copies.
        /// </summary>
        [Required(ErrorMessage = "Le nombre total de copies est obligatoire.")]
        public int TotalCopies { get; set; }

        /// <summary>
        /// Date de création de l'enregistrement.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de mise à jour de l'enregistrement.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
