using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using OrderService.Application.Interfaces;

public class GetPagedOrdersHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDistributedCache _cache;

    public GetPagedOrdersHandler(
        IOrderRepository orderRepository,
        IDistributedCache cache)
    {
        _orderRepository = orderRepository;
        _cache = cache;
    }

    public async Task<GetPagedOrdersResponse> Handle(
        GetPagedOrdersRequest request)
    {
        var cacheKey =
            $"orders:page:{request.Page}:size:{request.PageSize}";

        // 1. Redis'ten kontrol et
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (cachedData is not null)
        {
            return JsonSerializer.Deserialize<GetPagedOrdersResponse>(
                cachedData)!;
        }

        // 2. Cache'de yoksa PostgreSQL'den getir
        var orders = await _orderRepository.GetPagedAsync(
            request.Page,
            request.PageSize);

        var totalCount = await _orderRepository.CountAsync();

        var response = new GetPagedOrdersResponse
        {
            Items = orders,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages =
                (int)Math.Ceiling(
                    (double)totalCount / request.PageSize)
        };

        // 3. Redis'e yaz
        var serializedResponse =
            JsonSerializer.Serialize(response);

        await _cache.SetStringAsync(
            cacheKey,
            serializedResponse,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(5)
            });

        return response;
    }
}