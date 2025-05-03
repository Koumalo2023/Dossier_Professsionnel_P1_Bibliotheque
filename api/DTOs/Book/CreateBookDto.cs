using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    /// <summary>
    /// DTO pour la création d'un nouveau livre.
    /// </summary>
    public class CreateBookDto
    {
        [Required(ErrorMessage = "Le titre est obligatoire.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "L'auteur est obligatoire.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "La catégorie est obligatoire.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "L'ISBN est obligatoire.")]
        public string Isbn { get; set; }

        public DateTime? PublicationDate { get; set; }

        public string CoverUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Le nombre total de copies doit être supérieur à 0.")]
        public int TotalCopies { get; set; }
    }
}
