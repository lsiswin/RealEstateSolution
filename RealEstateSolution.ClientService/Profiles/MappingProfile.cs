using AutoMapper;
using RealEstateSolution.ClientService.Models;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Profiles
{
    /// <summary>
    /// AutoMapper配置文件
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 客户需求映射
            CreateMap<ClientRequirements, ClientRequirement>()
                .ReverseMap();
        }
    }
} 