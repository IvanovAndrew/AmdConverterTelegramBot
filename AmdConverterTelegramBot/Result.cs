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

}