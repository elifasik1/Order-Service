using Domain.Entities;

public class GetPagedOrdersResponse
{
    public List<Order> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}