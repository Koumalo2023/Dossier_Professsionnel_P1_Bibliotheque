using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// DTO pour créer une nouvelle catégorie.
    /// </summary>
    public class CreateCategoryDto
    {
        /// <summary>
        /// Nom de la catégorie.
        /// </summary>
        [Required(ErrorMessage = "Le nom de la catégorie est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Description de la catégorie.
        /// </summary>
        [MaxLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string? Description { get; set; }

        /// <summary>
        /// Icône associée à la catégorie.
        /// </summary>
        public string? Icon { get; set; }
    }
}