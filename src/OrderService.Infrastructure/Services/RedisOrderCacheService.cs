using OrderService.Application.Interfaces;
using StackExchange.Redis;

namespace OrderService.Infrastructure.Services;

public class RedisOrderCacheService : IOrderCacheService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisOrderCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task InvalidateOrdersCacheAsync()
    {
        var server = _redis.GetServer(
            _redis.GetEndPoints().First());

        var keys = server.Keys(
            pattern: "OrderService:orders:*");

        foreach (var key in keys)
        {
            await _redis.GetDatabase().KeyDeleteAsync(key);
        }
    }
}