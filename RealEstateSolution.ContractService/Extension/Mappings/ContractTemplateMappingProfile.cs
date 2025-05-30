using AutoMapper;
using RealEstateSolution.Database.Models;
using RealEstateSolution.ContractService.Models;

namespace RealEstateSolution.ContractService.Extension.Mappings;

/// <summary>
/// 合同模板映射配置
/// </summary>
public class ContractTemplateMappingProfile : Profile
{
    public ContractTemplateMappingProfile()
    {
        // ContractTemplate -> ContractTemplateDto
        CreateMap<ContractTemplate, ContractTemplateDto>();

        // ContractTemplateCreateDto -> ContractTemplate
        CreateMap<ContractTemplateCreateDto, ContractTemplate>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // 由服务设置
            .ForMember(dest => dest.FileSize, opt => opt.Ignore()); // 由服务计算
    }
} 