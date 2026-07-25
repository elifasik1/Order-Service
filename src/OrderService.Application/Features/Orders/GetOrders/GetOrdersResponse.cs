using Domain.Entities;

public class GetOrdersResponse
{
    public List<Order> Orders { get; set; } = new();
}