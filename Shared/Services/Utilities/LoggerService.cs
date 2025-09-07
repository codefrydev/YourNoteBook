using Microsoft.Extensions.Logging;

namespace YourNoteBook.Shared.Services.Utilities;

/// <summary>
/// Implementation of the logger service using Microsoft.Extensions.Logging
/// </summary>
public class LoggerService : ILoggerService
{
    private readonly ILogger<LoggerService> _logger;

    public LoggerService(ILogger<LoggerService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    /// <inheritdoc />
    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    /// <inheritdoc />
    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }

    /// <inheritdoc />
    public void LogError(Exception exception, string message, params object[] args)
    {
        _logger.LogError(exception, message, args);
    }

    /// <inheritdoc />
    public void LogDebug(string message, params object[] args)
    {
        _logger.LogDebug(message, args);
    }
}
