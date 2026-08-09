using AutoMapper;
using Domain.Entities;
namespace OrderService.Application.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
{
    CreateMap<Order, CreateOrderResponse>()
        .ForMember(
            dest => dest.Message,
            opt => opt.MapFrom(src => "Siparişiniz oluşturuldu.")
        );
}
}