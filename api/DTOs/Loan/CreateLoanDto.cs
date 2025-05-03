using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    /// <summary>
    /// DTO pour la création d'un nouvel emprunt.
    /// </summary>
    public class CreateLoanDto
    {
        [Required(ErrorMessage = "L'identifiant du livre est obligatoire.")]
        public Guid BookId { get; set; }

        [Required(ErrorMessage = "La date limite de retour est obligatoire.")]
        public DateTime DueDate { get; set; }
    }
}
