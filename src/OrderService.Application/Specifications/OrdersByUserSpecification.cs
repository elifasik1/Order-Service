using Domain.Entities;
using System.Linq.Expressions;

namespace OrderService.Application.Specifications;

public class OrdersByUserSpecification : ISpecification<Order>
{
    public Expression<Func<Order, bool>> Criteria { get; }

    public OrdersByUserSpecification(Guid userId)
    {
        Criteria = order => order.UserId == userId;
    }
}