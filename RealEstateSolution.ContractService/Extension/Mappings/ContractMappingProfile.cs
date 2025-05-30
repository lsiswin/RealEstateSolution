using AutoMapper;
using RealEstateSolution.Database.Models;
using RealEstateSolution.ContractService.Models;

namespace RealEstateSolution.ContractService.Extension.Mappings;

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
            .ForMember(dest => dest.PartyAId, opt => opt.MapFrom(src => src.ClientId))
            .ForMember(dest => dest.PartyAName, opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : null))
            .ForMember(dest => dest.PartyAPhone, opt => opt.MapFrom(src => src.Client != null ? src.Client.Phone : null))
            .ForMember(dest => dest.PartyBId, opt => opt.MapFrom(src => src.ClientId1 ?? 0))
            .ForMember(dest => dest.PartyBName, opt => opt.MapFrom(src => src.Client1 != null ? src.Client1.Name : null))
            .ForMember(dest => dest.PartyBPhone, opt => opt.MapFrom(src => src.Client1 != null ? src.Client1.Phone : null))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Terms.Length > 100 ? src.Terms.Substring(0, 100) : src.Terms))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => (DateTime?)src.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Terms, opt => opt.MapFrom(src => src.Terms))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Remark))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            .ForMember(dest => dest.SignDate, opt => opt.MapFrom(src => src.SignTime))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreateTime))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdateTime));

        // ContractDto -> Contract (用于创建和更新)
        CreateMap<ContractDto, Contract>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ContractNumber, opt => opt.Ignore()) // 将由服务生成
            .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.PartyAId))
            .ForMember(dest => dest.ClientId1, opt => opt.MapFrom(src => src.PartyBId == 0 ? (int?)null : src.PartyBId))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate ?? DateTime.Now))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            .ForMember(dest => dest.Terms, opt => opt.MapFrom(src => src.Terms ?? src.Title ?? string.Empty))
            .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod ?? string.Empty))
            .ForMember(dest => dest.SignTime, opt => opt.MapFrom(src => src.SignDate))
            .ForMember(dest => dest.CreateTime, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.UpdateTime, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.SignTime, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.CompleteTime, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.CancelTime, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.Property, opt => opt.Ignore()) // 导航属性
            .ForMember(dest => dest.Client, opt => opt.Ignore()) // 导航属性
            .ForMember(dest => dest.Client1, opt => opt.Ignore()); // 导航属性
    }
}