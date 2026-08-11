namespace OrderService.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public string Message { get; }

    protected Result(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result Success(string message = "")
    {
        return new Result(true, message);
    }

    public static Result Failure(string message)
    {
        return new Result(false, message);
    }
}