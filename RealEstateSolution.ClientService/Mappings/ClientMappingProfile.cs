using AutoMapper;
using RealEstateSolution.Database.Models;
using RealEstateSolution.ClientService.Dtos;

namespace RealEstateSolution.ClientService.Mappings;

/// <summary>
/// 客户映射配置 - 优化版本
/// </summary>
public class ClientMappingProfile : Profile
{
    public ClientMappingProfile()
    {
        ConfigureClientMappings();
        ConfigureClientRequirementMappings();
        ConfigureEnumMappings();
    }

    /// <summary>
    /// 配置客户映射
    /// </summary>
    private void ConfigureClientMappings()
    {
        // Client -> ClientDto (查询时使用)
        CreateMap<Client, ClientDto>()
            .ForMember(dest => dest.IdCard, opt => opt.Ignore()) // Client模型中没有IdCard字段
            .ForMember(dest => dest.Occupation, opt => opt.Ignore()) // Client模型中没有Occupation字段
            .ForMember(dest => dest.AnnualIncome, opt => opt.Ignore()) // Client模型中没有AnnualIncome字段
            .ForMember(dest => dest.Notes, opt => opt.Ignore()) // Client模型中没有Notes字段
            .ForMember(dest => dest.Requirements, opt => opt.Ignore()) // 需要单独查询
            .AfterMap((src, dest, context) => {
                // 后处理：格式化显示信息
                dest.DisplayName = FormatClientDisplayName(src.Name, src.Type);
                dest.StatusDisplay = GetStatusDisplayText(src.Status);
                dest.TypeDisplay = GetTypeDisplayText(src.Type);
            });

        // ClientDto -> Client (创建时使用)
        CreateMap<ClientDto, Client>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreateTime, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.UpdateTime, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.Source, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.Requirements, opt => opt.Ignore()) // 导航属性
            .ForMember(dest => dest.Matchings, opt => opt.Ignore()) // 导航属性
            .BeforeMap((src, dest, context) => {
                // 预处理：数据清理和验证
                if (!string.IsNullOrEmpty(src.Name))
                    src.Name = src.Name.Trim();
                if (!string.IsNullOrEmpty(src.Phone))
                    src.Phone = src.Phone.Trim();
                if (!string.IsNullOrEmpty(src.Email))
                    src.Email = src.Email.Trim();
                if (!string.IsNullOrEmpty(src.Address))
                    src.Address = src.Address.Trim();
            })
            .AfterMap((src, dest, context) => {
                // 后处理：设置默认值
                if (dest.Status == default)
                    dest.Status = ClientStatus.Potential;
            });

        // ClientDto -> Client (更新时使用) - 只映射允许更新的字段
        CreateMap<ClientDto, Client>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreateTime, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Source, opt => opt.Ignore()) // 来源不允许修改
            .ForMember(dest => dest.Requirements, opt => opt.Ignore())
            .ForMember(dest => dest.Matchings, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }

    /// <summary>
    /// 配置客户需求映射
    /// </summary>
    private void ConfigureClientRequirementMappings()
    {
        // ClientRequirements -> ClientRequirementDto
        CreateMap<ClientRequirements, ClientRequirementDto>()
            .AfterMap((src, dest, context) => {
                // 后处理：格式化价格和面积范围显示
                dest.PriceRangeDisplay = FormatPriceRange(src.MinPrice, src.MaxPrice);
                dest.AreaRangeDisplay = FormatAreaRange(src.MinArea, src.MaxArea);
                dest.PropertyTypeDisplay = src.PropertyType?.ToString() ?? "不限";
            });

        // ClientRequirementDto -> ClientRequirements
        CreateMap<ClientRequirementDto, ClientRequirements>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreateTime, opt => opt.Ignore())
            .ForMember(dest => dest.UpdateTime, opt => opt.Ignore())
            .ForMember(dest => dest.Client, opt => opt.Ignore())
            .BeforeMap((src, dest, context) => {
                // 预处理：数据验证
                if (src.MinPrice.HasValue && src.MaxPrice.HasValue && src.MinPrice > src.MaxPrice)
                {
                    throw new ArgumentException("最低价格不能大于最高价格");
                }
                if (src.MinArea.HasValue && src.MaxArea.HasValue && src.MinArea > src.MaxArea)
                {
                    throw new ArgumentException("最小面积不能大于最大面积");
                }
            });
    }

    /// <summary>
    /// 配置枚举映射
    /// </summary>
    private void ConfigureEnumMappings()
    {
        // 枚举到字符串的映射
        CreateMap<ClientType, string>().ConvertUsing(src => GetTypeDisplayText(src));
        CreateMap<ClientStatus, string>().ConvertUsing(src => GetStatusDisplayText(src));
        CreateMap<ClientSource, string>().ConvertUsing(src => GetSourceDisplayText(src));
    }

    #region 辅助方法

    /// <summary>
    /// 格式化客户显示名称
    /// </summary>
    private static string FormatClientDisplayName(string name, ClientType type)
    {
        var typeText = GetTypeDisplayText(type);
        return $"{name}({typeText})";
    }

    /// <summary>
    /// 获取客户类型显示文本
    /// </summary>
    private static string GetTypeDisplayText(ClientType type)
    {
        return type switch
        {
            ClientType.Buyer => "买家",
            ClientType.Seller => "卖家",
            ClientType.Tenant => "租客",
            ClientType.Landlord => "房东",
            _ => "未知"
        };
    }

    /// <summary>
    /// 获取客户状态显示文本
    /// </summary>
    private static string GetStatusDisplayText(ClientStatus status)
    {
        return status switch
        {
            ClientStatus.Potential => "潜在客户",
            ClientStatus.Active => "活跃客户",
            ClientStatus.Closed => "已成交",
            ClientStatus.Invalid => "无效客户",
            _ => "未知状态"
        };
    }

    /// <summary>
    /// 获取客户来源显示文本
    /// </summary>
    private static string GetSourceDisplayText(ClientSource source)
    {
        return source switch
        {
            ClientSource.Website => "网站注册",
            ClientSource.Phone => "电话咨询",
            ClientSource.Referral => "朋友推荐",
            ClientSource.Other => "其他渠道",
            _ => "未知来源"
        };
    }

    /// <summary>
    /// 格式化价格范围显示
    /// </summary>
    private static string FormatPriceRange(decimal? minPrice, decimal? maxPrice)
    {
        if (!minPrice.HasValue && !maxPrice.HasValue)
            return "价格不限";
        
        if (!minPrice.HasValue)
            return $"≤{FormatPrice(maxPrice.Value)}";
        
        if (!maxPrice.HasValue)
            return $"≥{FormatPrice(minPrice.Value)}";
        
        if (minPrice == maxPrice)
            return FormatPrice(minPrice.Value);
        
        return $"{FormatPrice(minPrice.Value)} - {FormatPrice(maxPrice.Value)}";
    }

    /// <summary>
    /// 格式化面积范围显示
    /// </summary>
    private static string FormatAreaRange(decimal? minArea, decimal? maxArea)
    {
        if (!minArea.HasValue && !maxArea.HasValue)
            return "面积不限";
        
        if (!minArea.HasValue)
            return $"≤{maxArea:F0}㎡";
        
        if (!maxArea.HasValue)
            return $"≥{minArea:F0}㎡";
        
        if (minArea == maxArea)
            return $"{minArea:F0}㎡";
        
        return $"{minArea:F0}㎡ - {maxArea:F0}㎡";
    }

    /// <summary>
    /// 格式化价格显示
    /// </summary>
    private static string FormatPrice(decimal price)
    {
        if (price >= 10000)
            return $"{price / 10000:F1}万";
        
        return $"{price:F0}元";
    }

    #endregion
} 