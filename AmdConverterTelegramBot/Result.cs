namespace AmdConverterTelegramBot;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Value { get; private set; }
    public string ErrorMessage { get; private set; }
    
    public static Result<T> Ok(T value)
    {
        return new Result<T>() { IsSuccess = true, Value = value };
    }

    public static Result<T> Error(string message)
    {
        return new Result<T>() { IsSuccess = false, ErrorMessage = message };
    }

    public Result<T2> Bind<T2>(Func<T, Result<T2>> bind)
    {
        return IsSuccess?
            bind(Value) :
            Result<T2>.Error(ErrorMessage);
    }

    public void IterValue(Action<T> action)
    {
        if (IsSuccess)
        {
            action(Value);
        }
    }
    
    public void IterError(Action<string> action)
    {
        if (!IsSuccess)
        {
            action(ErrorMessage);
        }
    }

    public T ValueOrDefault(T defaultValue)
    {
        return IsSuccess ? Value : defaultValue;
    }
}