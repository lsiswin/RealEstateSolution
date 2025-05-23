using AutoMapper;
using RealEstateSolution.ContractService.Dtos;
using RealEstateSolution.ContractService.Data;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ContractService.Extension
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Contract, ContractDto>();
            CreateMap<ContractDto, Contract>();
        }
    }
} 