using api.DTOs;
using api.Models;
using AutoMapper;

namespace api.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, ApplicationUser>().ReverseMap();
            CreateMap<BookDto, Book>().ReverseMap();
            CreateMap<LoanDto, Loan>().ReverseMap();
            CreateMap<NotificationDto, Notification>().ReverseMap();
        }
    }
}
