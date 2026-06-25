using AutoMapper;
using TruyenCV.Application.Modules.User.DTOs.Responses;
using TruyenCV.Domain.Entities;

namespace TruyenCV.Application.Modules.User.Mapping;

public class UserMappingProfile : AutoMapper.Profile
{
    public UserMappingProfile()
    {
        CreateMap<TruyenCV.Domain.Entities.User, UserResponse>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
