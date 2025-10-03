using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// DTO pour l'importation d'un livre depuis Google Books API
    /// </summary>
    public class ImportBookFromGoogleDto
    {
        /// <summary>
        /// ISBN du livre à importer (10 ou 13 chiffres)
        /// </summary>
        [Required(ErrorMessage = "L'ISBN est obligatoire pour l'importation.")]
        [RegularExpression(@"^(?:\d{10}|\d{13})$", ErrorMessage = "L'ISBN doit être au format 10 ou 13 chiffres.")]
        public string Isbn { get; set; } = string.Empty;

        /// <summary>
        /// Catégorie par défaut si non trouvée dans Google Books
        /// </summary>
        [Required(ErrorMessage = "La catégorie par défaut est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom de la catégorie ne peut pas dépasser 100 caractères.")]
        public string DefaultCategory { get; set; } = "Général";

        /// <summary>
        /// Nombre d'exemplaires à créer (par défaut 1)
        /// </summary>
        [Range(1, 1000, ErrorMessage = "Le nombre total de copies doit être compris entre 1 et 1000.")]
        public int TotalCopies { get; set; } = 1;
    }
}