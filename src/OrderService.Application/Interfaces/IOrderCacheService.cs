namespace OrderService.Application.Interfaces;

public interface IOrderCacheService
{
    Task InvalidateOrdersCacheAsync();
}