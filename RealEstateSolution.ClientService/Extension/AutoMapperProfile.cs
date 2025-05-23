using AutoMapper;
using RealEstateSolution.ClientService.Dtos;
using RealEstateSolution.ClientService.Data;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.ClientService.Extension
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Client, ClientDto>();
            CreateMap<ClientDto, Client>();
        }
    }
} 