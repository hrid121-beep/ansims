using AutoMapper;
using IMS.Application.DTOs;
using IMS.Domain.Entities;

namespace IMS.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Return mappings
            CreateMap<Return, ReturnDto>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item != null ? src.Item.Name : string.Empty))
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : string.Empty))
                .ReverseMap();

            CreateMap<ReturnDto, Return>()
                .ForMember(dest => dest.Item, opt => opt.Ignore())
                .ForMember(dest => dest.Store, opt => opt.Ignore());

            // Add more mappings as needed for other entities
        }
    }
}
