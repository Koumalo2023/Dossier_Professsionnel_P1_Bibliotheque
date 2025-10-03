using AutoMapper; 

namespace api.Models
{
    /// <summary>
    /// Profile AutoMapper pour les mappings entre l'entité AuditLog et ses DTOs.
    /// </summary>
    public class AuditLogProfile : Profile
    {
        public AuditLogProfile()
        {
            // Mapping de AuditLog vers AuditLogDto
            CreateMap<AuditLog, AuditLogDto>();

            // Mapping de CreateAuditLogDto vers AuditLog
            CreateMap<CreateAuditLogDto, AuditLog>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}