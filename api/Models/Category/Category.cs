using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models
{
    /// <summary>
    /// Représente une catégorie de livres dans la bibliothèque.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Identifiant unique de la catégorie (généré automatiquement).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Nom de la catégorie.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Description de la catégorie (optionnelle).
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Icône associée à la catégorie (ex: "bi-book" pour Bootstrap Icons).
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Date de création de la catégorie.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de mise à jour de la catégorie.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Livres appartenant à cette catégorie.
        /// </summary>
        public ICollection<Book> Books { get; set; } = new HashSet<Book>();

    }
}