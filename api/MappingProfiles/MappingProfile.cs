using api.DTOs;
using api.DTOs.ApplicationUser;
using api.DTOs.Book;
using api.DTOs.Loan;
using api.DTOs.Notification;
using api.Models;
using AutoMapper;

namespace api.MappingProfiles
{
    /// <summary>
    /// Profil de mapping pour AutoMapper.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mappings pour ApplicationUser
            CreateMap<ApplicationUser, UserDto>();
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
        }
    }
}
