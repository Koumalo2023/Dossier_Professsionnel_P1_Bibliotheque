using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    /// <summary>
    /// DTO pour la mise à jour d'un livre.
    /// </summary>
    public class UpdateBookDto
    {
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "L'auteur est obligatoire.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "La catégorie est obligatoire.")]
        public string Category { get; set; }

        public DateTime? PublicationDate { get; set; }

        public string CoverUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Le nombre total de copies doit être supérieur à 0.")]
        public int TotalCopies { get; set; }
    }
}
