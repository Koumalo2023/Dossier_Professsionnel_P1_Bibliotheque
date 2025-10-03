using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// DTO pour la création d'un nouveau livre.
    /// </summary>
    public class CreateBookDto
    {
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        [MaxLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'auteur est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom de l'auteur ne peut pas dépasser 100 caractères.")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "La catégorie est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le nom de la catégorie ne peut pas dépasser 100 caractères.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'ISBN est obligatoire.")]
        [RegularExpression(@"^(?:\d{10}|\d{13})$", ErrorMessage = "L'ISBN doit être au format 10 ou 13 chiffres.")]
        public string Isbn { get; set; } = string.Empty;

        public DateTime? PublicationDate { get; set; } = null;

        [Url(ErrorMessage = "L'URL de la couverture doit être une URL valide.")]
        [MaxLength(500, ErrorMessage = "L'URL de la couverture ne peut pas dépasser 500 caractères.")]
        public string CoverUrl { get; set; } = string.Empty;

        [MaxLength(2000, ErrorMessage = "La description ne peut pas dépasser 2000 caractères.")]
        public string Description { get; set; } = string.Empty;

        [Range(1, 10000, ErrorMessage = "Le nombre de pages doit être compris entre 1 et 10000.")]
        public int? PageCount { get; set; } = null;

        [MaxLength(200, ErrorMessage = "Le nom de l'éditeur ne peut pas dépasser 200 caractères.")]
        public string Publisher { get; set; } = string.Empty;

        [Range(1, 1000, ErrorMessage = "Le nombre total de copies doit être compris entre 1 et 1000.")]
        public int TotalCopies { get; set; } = 1;
    }
}
