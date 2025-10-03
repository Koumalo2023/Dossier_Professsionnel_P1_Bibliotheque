using AutoMapper; 

namespace api.Models
{
    /// <summary>
    /// Profile AutoMapper pour les mappings entre l'entité Notification et ses DTOs.
    /// </summary>
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            // Mapping de Notification vers NotificationDto
            CreateMap<Notification, NotificationDto>();

            // Mapping de CreateNotificationDto vers Notification
            CreateMap<CreateNotificationDto, Notification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Read, opt => opt.MapFrom(src => false));
        }
    }
}