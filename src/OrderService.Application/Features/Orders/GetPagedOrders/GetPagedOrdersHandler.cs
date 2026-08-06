using OrderService.Application.Interfaces;

public class GetPagedOrdersHandler
{
    private readonly IOrderRepository _orderRepository;
    public GetPagedOrdersHandler(IOrderRepository orderRepository)
{
    _orderRepository = orderRepository;
}
    public async Task<GetPagedOrdersResponse> Handle(GetPagedOrdersRequest request)
{

    var orders = await _orderRepository.GetPagedAsync(
        request.Page,
        request.PageSize);

    var totalCount = await _orderRepository.CountAsync();

    return new GetPagedOrdersResponse
    {
        Items = orders,
        Page = request.Page,
        PageSize = request.PageSize,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
    };
}
}