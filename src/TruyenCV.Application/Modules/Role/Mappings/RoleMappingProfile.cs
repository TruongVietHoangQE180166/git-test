using AutoMapper;
using TruyenCV.Application.Modules.Role.DTOs.Responses;
using TruyenCV.Domain.Entities;

namespace TruyenCV.Application.Modules.Role.Mappings;

public class RoleMappingProfile : AutoMapper.Profile
{
    public RoleMappingProfile()
    {
        CreateMap<TruyenCV.Domain.Entities.Role, RoleResponse>()
            .ForMember(dest => dest.UserCount, opt => opt.Ignore());
    }
}
