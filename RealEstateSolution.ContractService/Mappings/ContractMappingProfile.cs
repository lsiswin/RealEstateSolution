using AutoMapper;
using RealEstateSolution.Database.Models;
using RealEstateSolution.ContractService.Models;

namespace RealEstateSolution.ContractService.Mappings;

/// <summary>
/// 合同映射配置
/// </summary>
public class ContractMappingProfile : Profile
{
    public ContractMappingProfile()
    {
        // Contract -> ContractDto
        CreateMap<Contract, ContractDto>()
            .ForMember(dest => dest.PropertyTitle, opt => opt.MapFrom(src => src.Property != null ? src.Property.Title : null))
            .ForMember(dest => dest.PropertyAddress, opt => opt.MapFrom(src => src.Property != null ? src.Property.Address : null))
            .ForMember(dest => dest.PartyAName, opt => opt.MapFrom(src => src.PartyA != null ? src.PartyA.Name : null))
            .ForMember(dest => dest.PartyAPhone, opt => opt.MapFrom(src => src.PartyA != null ? src.PartyA.Phone : null))
            .ForMember(dest => dest.PartyBName, opt => opt.MapFrom(src => src.PartyB != null ? src.PartyB.Name : null))
            .ForMember(dest => dest.PartyBPhone, opt => opt.MapFrom(src => src.PartyB != null ? src.PartyB.Phone : null))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.EffectiveDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.ExpiryDate))
            .ForMember(dest => dest.Terms, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        // ContractDto -> Contract (用于创建和更新)
        CreateMap<ContractDto, Contract>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ContractNumber, opt => opt.Ignore()) // 将由服务生成
            .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.StartDate))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Terms))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.Property, opt => opt.Ignore()) // 导航属性
            .ForMember(dest => dest.PartyA, opt => opt.Ignore()) // 导航属性
            .ForMember(dest => dest.PartyB, opt => opt.Ignore()); // 导航属性
    }
} 