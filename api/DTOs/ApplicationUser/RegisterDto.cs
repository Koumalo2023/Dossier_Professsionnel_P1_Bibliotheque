using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    /// <summary>
    /// DTO pour l'enregistrement d'un nouvel utilisateur.
    /// </summary>
    public class RegisterDto
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'email doit être valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        public string Password { get; set; }
    }
}
