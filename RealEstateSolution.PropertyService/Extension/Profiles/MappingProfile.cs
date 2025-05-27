using AutoMapper;
using RealEstateSolution.PropertyService.Models;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.PropertyService.Extension.Profiles
{
    /// <summary>
    /// AutoMapper配置文件
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Property映射配置
            CreateMap<Property, Property>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // 忽略ID，避免覆盖
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore()) // 忽略所有者ID
                .ForMember(dest => dest.CreateTime, opt => opt.Ignore()) // 忽略创建时间
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => DateTime.Now)); // 设置更新时间

            // PropertyImage到PropertyImageDto映射
            CreateMap<PropertyImage, PropertyImageDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.FilePath))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.UploadTime, opt => opt.MapFrom(src => src.UploadedAt))
                .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => 0)); // 默认排序为0

            // PropertyImageDto到PropertyImage映射
            CreateMap<PropertyImageDto, PropertyImage>()
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.UploadedAt, opt => opt.MapFrom(src => src.UploadTime))
                .ForMember(dest => dest.FileSize, opt => opt.Ignore())
                .ForMember(dest => dest.FileType, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedBy, opt => opt.Ignore());
        }
    }
}