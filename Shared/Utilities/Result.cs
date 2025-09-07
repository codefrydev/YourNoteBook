namespace YourNoteBook.Shared.Utilities;

/// <summary>
/// Represents the result of an operation that can either succeed or fail
/// </summary>
/// <typeparam name="T">The type of the result value</typeparam>
public class Result<T>
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// The result value if the operation was successful
    /// </summary>
    public T? Value { get; }
    
    /// <summary>
    /// The error message if the operation failed
    /// </summary>
    public string? Error { get; }
    
    /// <summary>
    /// The exception if the operation failed with an exception
    /// </summary>
    public Exception? Exception { get; }

    private Result(bool isSuccess, T? value, string? error, Exception? exception)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Exception = exception;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <param name="value">The result value</param>
    /// <returns>A successful result</returns>
    public static Result<T> Success(T value) => new(true, value, null, null);

    /// <summary>
    /// Creates a failed result with an error message
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failed result</returns>
    public static Result<T> Failure(string error) => new(false, default, error, null);

    /// <summary>
    /// Creates a failed result with an exception
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns>A failed result</returns>
    public static Result<T> Failure(Exception exception) => new(false, default, exception.Message, exception);

    /// <summary>
    /// Creates a failed result with both an error message and exception
    /// </summary>
    /// <param name="error">The error message</param>
    /// <param name="exception">The exception</param>
    /// <returns>A failed result</returns>
    public static Result<T> Failure(string error, Exception exception) => new(false, default, error, exception);

    /// <summary>
    /// Implicitly converts a value to a successful result
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <returns>A successful result</returns>
    public static implicit operator Result<T>(T value) => Success(value);
}

/// <summary>
/// Represents the result of an operation that can either succeed or fail (without a value)
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// The error message if the operation failed
    /// </summary>
    public string? Error { get; }
    
    /// <summary>
    /// The exception if the operation failed with an exception
    /// </summary>
    public Exception? Exception { get; }

    private Result(bool isSuccess, string? error, Exception? exception)
    {
        IsSuccess = isSuccess;
        Error = error;
        Exception = exception;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <returns>A successful result</returns>
    public static Result Success() => new(true, null, null);

    /// <summary>
    /// Creates a failed result with an error message
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failed result</returns>
    public static Result Failure(string error) => new(false, error, null);

    /// <summary>
    /// Creates a failed result with an exception
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns>A failed result</returns>
    public static Result Failure(Exception exception) => new(false, exception.Message, exception);

    /// <summary>
    /// Creates a failed result with both an error message and exception
    /// </summary>
    /// <param name="error">The error message</param>
    /// <param name="exception">The exception</param>
    /// <returns>A failed result</returns>
    public static Result Failure(string error, Exception exception) => new(false, error, exception);
}
