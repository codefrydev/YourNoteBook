using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using YourNoteBook.Core.Entities;
using YourNoteBook.Shared.Services.DataManagement;

namespace YourNoteBook.Shared.Services.StateManagement;

/// <summary>
/// State manager interface for reactive state management
/// </summary>
public interface IStateManager : INotifyPropertyChanged, IDisposable
{
    // Application state
    bool IsInitialized { get; }
    bool IsLoading { get; }
    string? CurrentError { get; }
    string? CurrentFolderId { get; set; }
    string? CurrentNoteId { get; set; }
    
    // UI state
    bool IsSidebarVisible { get; set; }
    bool IsDarkMode { get; set; }
    string CurrentTheme { get; set; }
    
    // Events
    event Action<string>? OnError;
    event Action? OnStateChanged;
    event Action<string>? OnFolderChanged;
    event Action<string>? OnNoteChanged;
    
    // State management methods
    Task InitializeAsync();
    void SetError(string error);
    void ClearError();
    void SetLoading(bool loading);
    void SetCurrentFolder(string? folderId);
    void SetCurrentNote(string? noteId);
    void ToggleSidebar();
    void ToggleTheme();
}

/// <summary>
/// Implementation of the state manager for reactive state management
/// </summary>
public class StateManager : IStateManager, INotifyPropertyChanged
{
    private readonly IGlobalDataService _globalDataService;
    private bool _isInitialized;
    private bool _isLoading;
    private string? _currentError;
    private string? _currentFolderId;
    private string? _currentNoteId;
    private bool _isSidebarVisible = true;
    private bool _isDarkMode = false;
    private string _currentTheme = "light";

    public StateManager(IGlobalDataService globalDataService)
    {
        _globalDataService = globalDataService ?? throw new ArgumentNullException(nameof(globalDataService));
        
        // Subscribe to global data service events
        _globalDataService.OnDataChanged += OnGlobalDataChanged;
        _globalDataService.OnError += OnGlobalError;
    }

    // Application state properties
    public bool IsInitialized
    {
        get => _isInitialized;
        private set => SetProperty(ref _isInitialized, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public string? CurrentError
    {
        get => _currentError;
        private set => SetProperty(ref _currentError, value);
    }

    public string? CurrentFolderId
    {
        get => _currentFolderId;
        set => SetProperty(ref _currentFolderId, value, () => OnFolderChanged?.Invoke(value ?? ""));
    }

    public string? CurrentNoteId
    {
        get => _currentNoteId;
        set => SetProperty(ref _currentNoteId, value, () => OnNoteChanged?.Invoke(value ?? ""));
    }

    // UI state properties
    public bool IsSidebarVisible
    {
        get => _isSidebarVisible;
        set => SetProperty(ref _isSidebarVisible, value);
    }

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set => SetProperty(ref _isDarkMode, value);
    }

    public string CurrentTheme
    {
        get => _currentTheme;
        set => SetProperty(ref _currentTheme, value);
    }

    // Events
    public event Action<string>? OnError;
    public event Action? OnStateChanged;
    public event Action<string>? OnFolderChanged;
    public event Action<string>? OnNoteChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initialize the state manager
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            SetLoading(true);
            ClearError();
            
            // Initialize global data service if needed
            if (!_globalDataService.Notes.Any() && !_globalDataService.Folders.Any())
            {
                await _globalDataService.InitializeAsync();
            }
            
            IsInitialized = true;
            OnStateChanged?.Invoke();
        }
        catch (Exception ex)
        {
            SetError($"Failed to initialize state manager: {ex.Message}");
        }
        finally
        {
            SetLoading(false);
        }
    }

    /// <summary>
    /// Set an error state
    /// </summary>
    public void SetError(string error)
    {
        CurrentError = error;
        OnError?.Invoke(error);
        OnStateChanged?.Invoke();
    }

    /// <summary>
    /// Clear the current error
    /// </summary>
    public void ClearError()
    {
        CurrentError = null;
        OnStateChanged?.Invoke();
    }

    /// <summary>
    /// Set loading state
    /// </summary>
    public void SetLoading(bool loading)
    {
        IsLoading = loading;
        OnStateChanged?.Invoke();
    }

    /// <summary>
    /// Set the current folder
    /// </summary>
    public void SetCurrentFolder(string? folderId)
    {
        CurrentFolderId = folderId;
    }

    /// <summary>
    /// Set the current note
    /// </summary>
    public void SetCurrentNote(string? noteId)
    {
        CurrentNoteId = noteId;
    }

    /// <summary>
    /// Toggle sidebar visibility
    /// </summary>
    public void ToggleSidebar()
    {
        IsSidebarVisible = !IsSidebarVisible;
    }

    /// <summary>
    /// Toggle theme between light and dark
    /// </summary>
    public void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        CurrentTheme = IsDarkMode ? "dark" : "light";
    }

    // Private methods
    private void OnGlobalDataChanged()
    {
        OnStateChanged?.Invoke();
    }

    private void OnGlobalError(string error)
    {
        SetError(error);
    }

    private void SetProperty<T>(ref T field, T value, Action? onChanged = null, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        onChanged?.Invoke();
        OnStateChanged?.Invoke();
    }

    public void Dispose()
    {
        _globalDataService.OnDataChanged -= OnGlobalDataChanged;
        _globalDataService.OnError -= OnGlobalError;
    }
}

/// <summary>
/// Extension methods for state management
/// </summary>
public static class StateManagerExtensions
{
    /// <summary>
    /// Creates a reactive state binding
    /// </summary>
    public static ReactiveStateBinding<T> CreateStateBinding<T>(this IStateManager stateManager,
        Func<IStateManager, T> stateSelector,
        Action<T> onStateChanged)
    {
        return new ReactiveStateBinding<T>(stateManager, stateSelector, onStateChanged);
    }

    /// <summary>
    /// Creates a binding for the current folder
    /// </summary>
    public static ReactiveStateBinding<string?> CreateCurrentFolderBinding(this IStateManager stateManager,
        Action<string?> onFolderChanged)
    {
        return stateManager.CreateStateBinding(sm => sm.CurrentFolderId, onFolderChanged);
    }

    /// <summary>
    /// Creates a binding for the current note
    /// </summary>
    public static ReactiveStateBinding<string?> CreateCurrentNoteBinding(this IStateManager stateManager,
        Action<string?> onNoteChanged)
    {
        return stateManager.CreateStateBinding(sm => sm.CurrentNoteId, onNoteChanged);
    }

    /// <summary>
    /// Creates a binding for loading state
    /// </summary>
    public static ReactiveStateBinding<bool> CreateLoadingBinding(this IStateManager stateManager,
        Action<bool> onLoadingChanged)
    {
        return stateManager.CreateStateBinding(sm => sm.IsLoading, onLoadingChanged);
    }

    /// <summary>
    /// Creates a binding for error state
    /// </summary>
    public static ReactiveStateBinding<string?> CreateErrorBinding(this IStateManager stateManager,
        Action<string?> onErrorChanged)
    {
        return stateManager.CreateStateBinding(sm => sm.CurrentError, onErrorChanged);
    }
}

/// <summary>
/// Reactive state binding for automatic UI updates
/// </summary>
public class ReactiveStateBinding<T> : IDisposable
{
    private readonly IStateManager _stateManager;
    private readonly Action<T> _onStateChanged;
    private readonly Func<IStateManager, T> _stateSelector;

    public ReactiveStateBinding(IStateManager stateManager,
        Func<IStateManager, T> stateSelector,
        Action<T> onStateChanged)
    {
        _stateManager = stateManager;
        _stateSelector = stateSelector;
        _onStateChanged = onStateChanged;

        // Subscribe to state changes
        _stateManager.OnStateChanged += OnStateChanged;
        
        // Initial state load
        OnStateChanged();
    }

    private void OnStateChanged()
    {
        var state = _stateSelector(_stateManager);
        _onStateChanged(state);
    }

    public void Dispose()
    {
        _stateManager.OnStateChanged -= OnStateChanged;
    }
}
