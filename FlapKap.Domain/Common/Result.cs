namespace FlapKap.Domain.Common;

public class Result<T> : Result where T : class
{
    public T? Data { get; private set; }

    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static new Result<T> Failure(string error, T data = null)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Data = data,
            Errors = [error]
        };
    }

    public static new Result<T> Failure(List<string> errors, T data = null)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Data = data,
            Errors = errors
        };
    }
}

public class Result
{
    public bool IsSuccess { get; protected set; }
    public List<string> Errors { get; protected set; } = new();

    public static Result Success()
    {
        return new Result
        {
            IsSuccess = true
        };
    }

    public static Result Failure(string error)
    {
        return new Result
        {
            IsSuccess = false,
            Errors = [error] 
        };
    }

    public static Result Failure(List<string> errors)
    {
        return new Result
        {
            IsSuccess = false,
            Errors = errors
        };
    }
} 