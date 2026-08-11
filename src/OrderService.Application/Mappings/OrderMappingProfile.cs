using AutoMapper;
using Domain.Entities;

namespace OrderService.Application.Mappings;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, CreateOrderResponse>();
        CreateMap<Order, UpdateOrderResponse>();
    }
}