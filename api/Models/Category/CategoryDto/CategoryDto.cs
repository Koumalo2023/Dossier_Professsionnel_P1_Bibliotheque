using System;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// DTO pour représenter une catégorie de livres.
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// Identifiant unique de la catégorie.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nom de la catégorie.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Description de la catégorie.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Icône associée à la catégorie.
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// Nombre de livres dans cette catégorie.
        /// </summary>
        public int BookCount { get; set; }

        /// <summary>
        /// Date de création de la catégorie.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de mise à jour de la catégorie.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}