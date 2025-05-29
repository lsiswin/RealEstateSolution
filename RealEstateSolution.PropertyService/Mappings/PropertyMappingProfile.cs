using AutoMapper;
using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Dtos;
using System.IO;

namespace RealEstateSolution.PropertyService.Mappings;

/// <summary>
/// 房源映射配置 - 优化版本
/// </summary>
public class PropertyMappingProfile : Profile
{
    public PropertyMappingProfile()
    {
        ConfigurePropertyMappings();
        ConfigurePropertyImageMappings();
        ConfigureEnumMappings();
    }

    /// <summary>
    /// 配置房源映射
    /// </summary>
    private void ConfigurePropertyMappings()
    {
        // Property -> PropertyDto (查询时使用)
        CreateMap<Property, PropertyDto>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => ExtractCityFromAddress(src.Address)))
            .ForMember(dest => dest.District, opt => opt.MapFrom(src => ExtractDistrictFromAddress(src.Address)))
            .ForMember(dest => dest.Bedrooms, opt => opt.MapFrom(src => src.Rooms))
            .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => (int?)src.Floor))
            .ForMember(dest => dest.TotalFloors, opt => opt.MapFrom(src => (int?)src.TotalFloors))
            .ForMember(dest => dest.YearBuilt, opt => opt.MapFrom(src => ExtractYearFromDescription(src.Description)))
            .ForMember(dest => dest.Orientation, opt => opt.MapFrom(src => src.Orientation.ToString()))
            .ForMember(dest => dest.Decoration, opt => opt.MapFrom(src => src.Decoration.ToString()))
            .ForMember(dest => dest.Facilities, opt => opt.MapFrom(src => ExtractFacilitiesFromDescription(src.Description)))
            .ForMember(dest => dest.AgentId, opt => opt.MapFrom(src => src.OwnerId))
            .ForMember(dest => dest.Images, opt => opt.Ignore()) // 需要单独查询
            .AfterMap((src, dest, context) => {
                // 后处理：格式化价格显示等
                if (dest.Price > 10000)
                {
                    dest.PriceDisplay = $"{dest.Price / 10000:F1}万";
                }
                else
                {
                    dest.PriceDisplay = $"{dest.Price:F0}元";
                }
            });

        // PropertyDto -> Property (创建时使用)
        CreateMap<PropertyDto, Property>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreateTime, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateTime, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src => src.Bedrooms ?? 0))
            .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.Floor ?? 0))
            .ForMember(dest => dest.TotalFloors, opt => opt.MapFrom(src => src.TotalFloors ?? 0))
            .ForMember(dest => dest.Orientation, opt => opt.MapFrom(src => ParseOrientation(src.Orientation)))
            .ForMember(dest => dest.Decoration, opt => opt.MapFrom(src => ParseDecoration(src.Decoration)))
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .BeforeMap((src, dest, context) => {
                // 预处理：数据清理和验证
                if (!string.IsNullOrEmpty(src.Title))
                    src.Title = src.Title.Trim();
                if (!string.IsNullOrEmpty(src.Address))
                    src.Address = src.Address.Trim();
                if (!string.IsNullOrEmpty(src.Description))
                    src.Description = src.Description.Trim();
            });

        // PropertyDto -> Property (更新时使用) - 只映射允许更新的字段
        CreateMap<PropertyDto, Property>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreateTime, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateTime, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore()) // 状态单独更新
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src => src.Bedrooms ?? 0))
            .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.Floor ?? 0))
            .ForMember(dest => dest.TotalFloors, opt => opt.MapFrom(src => src.TotalFloors ?? 0))
            .ForMember(dest => dest.Orientation, opt => opt.MapFrom(src => ParseOrientation(src.Orientation)))
            .ForMember(dest => dest.Decoration, opt => opt.MapFrom(src => ParseDecoration(src.Decoration)))
            .ConstructUsing((src, context) => context.Mapper.Map<Property>(src))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }

    /// <summary>
    /// 配置房源图片映射
    /// </summary>
    private void ConfigurePropertyImageMappings()
    {
        // PropertyImage -> PropertyImageDto
        CreateMap<PropertyImage, PropertyImageDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => FormatImageUrl(src.FilePath)))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => GenerateImageDescription(src.FileName, src.IsMain)))
            .ForMember(dest => dest.IsMain, opt => opt.MapFrom(src => src.IsMain))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.IsMain ? 0 : 1))
            .ForMember(dest => dest.UploadTime, opt => opt.MapFrom(src => src.UploadedAt))
            .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => FormatFileSize(src.FileSize)))
            .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => GenerateThumbnailUrl(src.FilePath)));

        // PropertyImageDto -> PropertyImage
        CreateMap<PropertyImageDto, PropertyImage>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PropertyId, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.FileName, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.FileSize, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.FileType, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.UploadedAt, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.UploadedBy, opt => opt.Ignore()); // 由服务设置
    }

    /// <summary>
    /// 配置枚举映射
    /// </summary>
    private void ConfigureEnumMappings()
    {
        // 字符串到枚举的映射
        CreateMap<string, OrientationType>().ConvertUsing(src => ParseOrientation(src));
        CreateMap<string, DecorationType>().ConvertUsing(src => ParseDecoration(src));
        
        // 枚举到字符串的映射
        CreateMap<OrientationType, string>().ConvertUsing(src => src.ToString());
        CreateMap<DecorationType, string>().ConvertUsing(src => src.ToString());
    }

    #region 辅助方法

    /// <summary>
    /// 从地址中提取城市信息
    /// </summary>
    private static string? ExtractCityFromAddress(string? address)
    {
        if (string.IsNullOrEmpty(address)) return null;
        
        // 简单的城市提取逻辑，可以根据实际需求优化
        var parts = address.Split(new[] { "省", "市", "区", "县" }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? parts[1] : null;
    }

    /// <summary>
    /// 从地址中提取区县信息
    /// </summary>
    private static string? ExtractDistrictFromAddress(string? address)
    {
        if (string.IsNullOrEmpty(address)) return null;
        
        // 简单的区县提取逻辑
        var parts = address.Split(new[] { "区", "县" }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0].Split(new[] { "市" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault() : null;
    }

    /// <summary>
    /// 从描述中提取建造年份
    /// </summary>
    private static int? ExtractYearFromDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return null;
        
        // 使用正则表达式提取年份
        var yearMatch = System.Text.RegularExpressions.Regex.Match(description, @"(\d{4})年");
        if (yearMatch.Success && int.TryParse(yearMatch.Groups[1].Value, out var year))
        {
            return year >= 1900 && year <= DateTime.Now.Year ? year : null;
        }
        return null;
    }

    /// <summary>
    /// 从描述中提取设施信息
    /// </summary>
    private static List<string>? ExtractFacilitiesFromDescription(string? description)
    {
        if (string.IsNullOrEmpty(description)) return null;
        
        var facilities = new List<string>();
        var facilityKeywords = new[] { "电梯", "停车位", "阳台", "花园", "游泳池", "健身房", "地暖", "中央空调" };
        
        foreach (var keyword in facilityKeywords)
        {
            if (description.Contains(keyword))
            {
                facilities.Add(keyword);
            }
        }
        
        return facilities.Any() ? facilities : null;
    }

    /// <summary>
    /// 格式化图片URL
    /// </summary>
    private static string FormatImageUrl(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return string.Empty;
        
        // 确保URL格式正确
        return filePath.StartsWith("http") ? filePath : $"/uploads/{filePath.TrimStart('/')}";
    }

    /// <summary>
    /// 生成图片描述
    /// </summary>
    private static string GenerateImageDescription(string? fileName, bool isMain)
    {
        if (string.IsNullOrEmpty(fileName)) return string.Empty;
        
        var baseName = Path.GetFileNameWithoutExtension(fileName);
        return isMain ? $"主图 - {baseName}" : $"房源图片 - {baseName}";
    }

    /// <summary>
    /// 格式化文件大小
    /// </summary>
    private static string FormatFileSize(long fileSize)
    {
        if (fileSize < 1024) return $"{fileSize} B";
        if (fileSize < 1024 * 1024) return $"{fileSize / 1024.0:F1} KB";
        return $"{fileSize / (1024.0 * 1024.0):F1} MB";
    }

    /// <summary>
    /// 生成缩略图URL
    /// </summary>
    private static string GenerateThumbnailUrl(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return string.Empty;
        
        var directory = Path.GetDirectoryName(filePath);
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var extension = Path.GetExtension(filePath);
        
        return $"{directory}/thumb_{fileName}{extension}";
    }

    /// <summary>
    /// 解析朝向字符串为枚举
    /// </summary>
    private static OrientationType ParseOrientation(string? orientation)
    {
        if (string.IsNullOrEmpty(orientation))
            return OrientationType.South;

        return orientation.ToLower() switch
        {
            "north" or "北" => OrientationType.North,
            "south" or "南" => OrientationType.South,
            "east" or "东" => OrientationType.East,
            "west" or "西" => OrientationType.West,
            "northeast" or "东北" => OrientationType.Northeast,
            "northwest" or "西北" => OrientationType.Northwest,
            "southeast" or "东南" => OrientationType.Southeast,
            "southwest" or "西南" => OrientationType.Southwest,
            _ => OrientationType.South
        };
    }

    /// <summary>
    /// 解析装修字符串为枚举
    /// </summary>
    private static DecorationType ParseDecoration(string? decoration)
    {
        if (string.IsNullOrEmpty(decoration))
            return DecorationType.Simple;

        return decoration.ToLower() switch
        {
            "simple" or "简装" => DecorationType.Simple,
            "fine" or "精装" => DecorationType.Fine,
            "luxury" or "豪装" => DecorationType.Luxury,
            "rough" or "毛坯" => DecorationType.Rough,
            _ => DecorationType.Simple
        };
    }

    #endregion
}