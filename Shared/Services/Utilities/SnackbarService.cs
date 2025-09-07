using System.ComponentModel;

namespace YourNoteBook.Shared.Services.Utilities;

public class SnackbarMessage
{
    public string Message { get; set; } = string.Empty;
    public SnackbarType Type { get; set; } = SnackbarType.Info;
    public int Duration { get; set; } = 4000; // Duration in milliseconds
    public bool IsVisible { get; set; } = false;
}

public enum SnackbarType
{
    Success,
    Error,
    Warning,
    Info
}

public class SnackbarService : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<SnackbarMessage>? OnShow;

    private SnackbarMessage _currentMessage = new();

    public SnackbarMessage CurrentMessage
    {
        get => _currentMessage;
        private set
        {
            _currentMessage = value;
            OnPropertyChanged();
        }
    }

    public void ShowSuccess(string message, int duration = 4000)
    {
        Show(message, SnackbarType.Success, duration);
    }

    public void ShowError(string message, int duration = 6000)
    {
        Show(message, SnackbarType.Error, duration);
    }

    public void ShowWarning(string message, int duration = 5000)
    {
        Show(message, SnackbarType.Warning, duration);
    }

    public void ShowInfo(string message, int duration = 4000)
    {
        Show(message, SnackbarType.Info, duration);
    }

    private void Show(string message, SnackbarType type, int duration)
    {
        Console.WriteLine($"[SnackbarService] Showing message: '{message}', Type: {type}, Duration: {duration}ms");
        
        var snackbarMessage = new SnackbarMessage
        {
            Message = message,
            Type = type,
            Duration = duration,
            IsVisible = true
        };

        CurrentMessage = snackbarMessage;
        Console.WriteLine($"[SnackbarService] CurrentMessage set, IsVisible: {CurrentMessage.IsVisible}");
        Console.WriteLine($"[SnackbarService] OnShow event subscribers: {OnShow?.GetInvocationList()?.Length ?? 0}");
        
        OnShow?.Invoke(snackbarMessage);
        Console.WriteLine($"[SnackbarService] OnShow event invoked");
    }

    public void Hide()
    {
        var hiddenMessage = new SnackbarMessage
        {
            Message = CurrentMessage.Message,
            Type = CurrentMessage.Type,
            Duration = CurrentMessage.Duration,
            IsVisible = false
        };

        CurrentMessage = hiddenMessage;
    }

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
