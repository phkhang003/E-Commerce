using AutoMapper;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Entities;

namespace ECommerce.WebAPI.Mappings
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<Cart, CartDto>();
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, 
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.UnitPrice,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0))
                .ForMember(dest => dest.SubTotal,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Price * src.Quantity : 0));
        }
    }
}