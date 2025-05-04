using api.DTOs.ApplicationUser;
using api.Models;
using AutoMapper;

namespace api.MappingProfiles
{
    // Profiles/UserProfile.cs
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Roles,
                           opt => opt.MapFrom(src => src.GetRolesList()));

            CreateMap<UserDto, ApplicationUser>()
                .ForMember(dest => dest.Roles,
                           opt => opt.MapFrom(src => string.Join(",", src.Roles)));
        }
    }
}
