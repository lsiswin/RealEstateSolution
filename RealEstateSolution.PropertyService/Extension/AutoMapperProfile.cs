using AutoMapper;
using RealEstateSolution.Database.Models;
using RealEstateSolution.PropertyService.Data.Dtos;

namespace RealEstateSolution.PropertyService.Extension
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Property, PropertyDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Decoration, opt => opt.MapFrom(src => src.Decoration))
                .ForMember(dest => dest.Orientation, opt => opt.MapFrom(src => src.Orientation))
                .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.Floor))
                .ForMember(dest => dest.TotalFloors, opt => opt.MapFrom(src => src.TotalFloors))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src => src.Rooms))
                .ForMember(dest => dest.Bathrooms, opt => opt.MapFrom(src => src.Bathrooms))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(pi => pi.ImageUrl).ToList()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreateTime))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdateTime));

            CreateMap<PropertyDto, Property>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Decoration, opt => opt.MapFrom(src => src.Decoration))
                .ForMember(dest => dest.Orientation, opt => opt.MapFrom(src => src.Orientation))
                .ForMember(dest => dest.Floor, opt => opt.MapFrom(src => src.Floor))
                .ForMember(dest => dest.TotalFloors, opt => opt.MapFrom(src => src.TotalFloors))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src => src.Rooms))
                .ForMember(dest => dest.Bathrooms, opt => opt.MapFrom(src => src.Bathrooms))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.UpdatedAt));
        }
    }
}