using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// DTO pour la mise à jour du profil utilisateur.
    /// </summary>
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "L'email doit être valide.")]
        public string Email { get; set; }
    }
}
