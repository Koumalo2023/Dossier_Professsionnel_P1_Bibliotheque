using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    /// <summary>
    /// DTO pour la connexion d'un utilisateur.
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'email doit être valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        public string Password { get; set; }
    }
}
