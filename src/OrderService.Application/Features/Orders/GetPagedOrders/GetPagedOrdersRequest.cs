

using FluentValidation;

public class GetPagedOrdersRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}

public class GetPagedOrdersValidator : AbstractValidator<GetPagedOrdersRequest>
{
    public GetPagedOrdersValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}