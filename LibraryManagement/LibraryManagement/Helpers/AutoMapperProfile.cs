using AutoMapper;
using LibraryManagement.DTOs.Auth;
using LibraryManagement.DTOs.User;
using LibraryManagement.Models;

namespace LibraryManagement.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<User, UserResponse>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<CreateUserRequest, User>();

            // Auth mappings
            CreateMap<LoginRequest, User>();
        }
    }
}