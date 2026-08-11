using System.Linq.Expressions;

namespace OrderService.Application.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
}