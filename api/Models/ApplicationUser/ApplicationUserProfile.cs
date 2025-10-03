using AutoMapper;

namespace api.Models
{
    /// <summary>
    /// Profil de mapping AutoMapper pour les entités ApplicationUser et DTOs associés
    /// </summary>
    public class ApplicationUserProfile : Profile
    {
        public ApplicationUserProfile()
        {
            // Mappings pour ApplicationUser
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Roles,
                           opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()));

            CreateMap<UserDto, ApplicationUser>()
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<UpdateUserDto, ApplicationUser>();

            // Mappings pour Book
            CreateMap<Book, BookDto>();
            CreateMap<CreateBookDto, Book>();
            CreateMap<UpdateBookDto, Book>();

            // Mappings pour Loan
            CreateMap<Loan, LoanDto>();
            CreateMap<CreateLoanDto, Loan>();
            CreateMap<UpdateLoanDto, Loan>();

            // Mappings pour Notification
            CreateMap<Notification, NotificationDto>();
            CreateMap<CreateNotificationDto, Notification>();

            // Mappings pour Category
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();

            // Mappings pour AnalyticsLog
            CreateMap<AnalyticsLog, AnalyticsLogDto>();
            CreateMap<CreateAnalyticsLogDto, AnalyticsLog>();

            // Mappings pour AuditLog
            CreateMap<AuditLog, AuditLogDto>();
            CreateMap<CreateAuditLogDto, AuditLog>();

            // Mappings pour Reservation
            CreateMap<Reservation, ReservationDto>();
            CreateMap<CreateReservationDto, Reservation>();
            CreateMap<UpdateReservationDto, Reservation>();
        }
    }
}