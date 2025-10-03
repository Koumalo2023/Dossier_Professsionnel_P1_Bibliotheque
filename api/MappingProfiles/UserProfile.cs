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
                           opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()));

            // Remove reverse mapping for now - will need to handle role assignment differently
            CreateMap<UserDto, ApplicationUser>()
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());
        }
    }
}
