using AutoMapper;
using RealEstateSolution.MatchingService.Dtos;
using RealEstateSolution.MatchingService.Data;
using RealEstateSolution.Database.Models;

namespace RealEstateSolution.MatchingService.Extension
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Matching, MatchingDto>();
            CreateMap<MatchingDto, Matching>();
        }
    }
} 