using AutoMapper;
using api.Models;

namespace api.Models
{
    /// <summary>
    /// Profile AutoMapper pour les mappings entre l'entité AnalyticsLog et ses DTOs.
    /// </summary>
    public class AnalyticsLogProfile : Profile
    {
        public AnalyticsLogProfile()
        {
            // Mapping de AnalyticsLog vers AnalyticsLogDto
            CreateMap<AnalyticsLog, AnalyticsLogDto>()
                .ForMember(dest => dest.BookTitle, opt => opt.Ignore()) // Rempli dans le service si applicable
                .ForMember(dest => dest.UserName, opt => opt.Ignore()); // Rempli dans le service si applicable

            // Mapping de CreateAnalyticsLogDto vers AnalyticsLog
            CreateMap<CreateAnalyticsLogDto, AnalyticsLog>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}