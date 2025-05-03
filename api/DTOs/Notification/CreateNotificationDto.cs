using System.ComponentModel.DataAnnotations;

namespace api.DTOs
{
    /// <summary>
    /// DTO pour la création d'une notification.
    /// </summary>
    public class CreateNotificationDto
    {
        [Required(ErrorMessage = "L'identifiant de l'utilisateur est obligatoire.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Le message est obligatoire.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Le type est obligatoire.")]
        public string Type { get; set; }
    }
}
