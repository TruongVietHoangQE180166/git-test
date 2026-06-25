using AutoMapper;
using TruyenCV.Application.Modules.Auth.DTOs.Responses;
using TruyenCV.Domain.Entities;

namespace TruyenCV.Application.Modules.Auth.Mappings;

public class AuthMappingProfile : AutoMapper.Profile
{
    public AuthMappingProfile()
    {
        // Auth Mappings
        CreateMap<TruyenCV.Domain.Entities.User, AuthResponse.UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : "Unknown"));
    }
}
