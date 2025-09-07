namespace YourNoteBook.Shared.Services.Utilities;

/// <summary>
/// Service for logging application events and errors
/// </summary>
public interface ILoggerService
{
    /// <summary>
    /// Logs an information message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="args">Optional format arguments</param>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Logs a warning message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="args">Optional format arguments</param>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Logs an error message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="args">Optional format arguments</param>
    void LogError(string message, params object[] args);

    /// <summary>
    /// Logs an error with an exception
    /// </summary>
    /// <param name="exception">The exception to log</param>
    /// <param name="message">The message to log</param>
    /// <param name="args">Optional format arguments</param>
    void LogError(Exception exception, string message, params object[] args);

    /// <summary>
    /// Logs a debug message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="args">Optional format arguments</param>
    void LogDebug(string message, params object[] args);
}
