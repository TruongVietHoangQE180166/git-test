using AutoMapper;
using TruyenCV.Application.Modules.Profile.DTOs.Responses;
using TruyenCV.Domain.Entities;

namespace TruyenCV.Application.Modules.Profile.Mappings;

public class ProfileMappingProfile : AutoMapper.Profile
{
    public ProfileMappingProfile()
    {
        CreateMap<TruyenCV.Domain.Entities.Profile, ProfileResponse>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty));
    }
}
