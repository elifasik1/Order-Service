namespace OrderService.Application.Common;

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(bool isSuccess, string message, T? data)
        : base(isSuccess, message)
    {
        Data = data;
    }

  public static Result<T> Success(T data, string message = "")
{
    return new Result<T>(true, message, data);
}

public static new Result<T> Failure(string message)
{
    return new Result<T>(false, message, default);
}
}