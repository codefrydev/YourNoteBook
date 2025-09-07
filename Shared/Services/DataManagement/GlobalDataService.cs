using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using YourNoteBook.Core.Interfaces;
using YourNoteBook.Core.Entities;
using YourNoteBook.Infrastructure.Data.Context;

namespace YourNoteBook.Shared.Services.DataManagement;

/// <summary>
/// Global data service interface for centralized data management and UI updates
/// </summary>
public interface IGlobalDataService : INotifyPropertyChanged, IDisposable
{
    // Observable collections for reactive UI updates
    ObservableCollection<Note> Notes { get; }
    ObservableCollection<Folder> Folders { get; }
    ObservableCollection<Shortcut> Shortcuts { get; }
    ObservableCollection<Tag> Tags { get; }
    ObservableCollection<Category> Categories { get; }

    // Data access methods
    Task InitializeAsync();
    Task RefreshAllDataAsync();
    Task RefreshNotesAsync();
    Task RefreshFoldersAsync();
    Task RefreshShortcutsAsync();
    Task RefreshTagsAsync();
    Task RefreshCategoriesAsync();

    // CRUD operations with automatic UI updates
    Task<Note> AddNoteAsync(Note note);
    Task<Note> UpdateNoteAsync(Note note);
    Task DeleteNoteAsync(string noteId);
    
    Task<Folder> AddFolderAsync(Folder folder);
    Task<Folder> UpdateFolderAsync(Folder folder);
    Task DeleteFolderAsync(string folderId);
    
    Task<Shortcut> AddShortcutAsync(Shortcut shortcut);
    Task<Shortcut> UpdateShortcutAsync(Shortcut shortcut);
    Task DeleteShortcutAsync(string shortcutId);
    
    Task<Tag> AddTagAsync(Tag tag);
    Task<Tag> UpdateTagAsync(Tag tag);
    Task DeleteTagAsync(string tagId);
    
    Task<Category> AddCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(string categoryId);

    // Query methods
    List<Note> GetNotesByFolderId(string folderId);
    List<Shortcut> GetShortcutsByFolderId(string folderId);
    List<Note> GetNotesByTagId(string tagId);
    Note? GetNoteById(string id);
    Folder? GetFolderById(string id);
    Shortcut? GetShortcutById(string id);
    Tag? GetTagById(string id);
    Category? GetCategoryById(string id);

    // State management
    bool IsLoading { get; }
    string? LastError { get; }
    event Action<string>? OnError;
    event Action? OnDataChanged;
}

/// <summary>
/// Implementation of the global data service with reactive UI updates
/// </summary>
public class GlobalDataService : IGlobalDataService, INotifyPropertyChanged
{
    private readonly IManagerFactory _managerFactory;
    private readonly InMemoryRepo _inMemoryRepo;
    private bool _isLoading;
    private string? _lastError;

    public GlobalDataService(IManagerFactory managerFactory, InMemoryRepo inMemoryRepo)
    {
        _managerFactory = managerFactory ?? throw new ArgumentNullException(nameof(managerFactory));
        _inMemoryRepo = inMemoryRepo ?? throw new ArgumentNullException(nameof(inMemoryRepo));

        // Initialize observable collections
        Notes = new ObservableCollection<Note>();
        Folders = new ObservableCollection<Folder>();
        Shortcuts = new ObservableCollection<Shortcut>();
        Tags = new ObservableCollection<Tag>();
        Categories = new ObservableCollection<Category>();

        // Subscribe to in-memory repo changes
        _inMemoryRepo.OnChange += OnInMemoryDataChanged;
    }

    // Observable collections for reactive UI updates
    public ObservableCollection<Note> Notes { get; }
    public ObservableCollection<Folder> Folders { get; }
    public ObservableCollection<Shortcut> Shortcuts { get; }
    public ObservableCollection<Tag> Tags { get; }
    public ObservableCollection<Category> Categories { get; }

    // State properties
    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public string? LastError
    {
        get => _lastError;
        private set => SetProperty(ref _lastError, value);
    }

    // Events
    public event Action<string>? OnError;
    public event Action? OnDataChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initialize the service and load all data
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            IsLoading = true;
            LastError = null;
            await RefreshAllDataAsync();
        }
        catch (Exception ex)
        {
            HandleError($"Failed to initialize data service: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Refresh all data from the data source
    /// </summary>
    public async Task RefreshAllDataAsync()
    {
        try
        {
            IsLoading = true;
            LastError = null;

            await Task.WhenAll(
                RefreshNotesAsync(),
                RefreshFoldersAsync(),
                RefreshShortcutsAsync(),
                RefreshTagsAsync(),
                RefreshCategoriesAsync()
            );

            OnDataChanged?.Invoke();
        }
        catch (Exception ex)
        {
            HandleError($"Failed to refresh all data: {ex.Message}", ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Refresh notes data
    /// </summary>
    public async Task RefreshNotesAsync()
    {
        try
        {
            var notesManager = _managerFactory.CreateNotesManager();
            var notes = await notesManager.GetAllSync();
            
            Notes.Clear();
            foreach (var note in notes)
            {
                Notes.Add(note);
            }

            // Update in-memory repo
            _inMemoryRepo.Notes.Clear();
            _inMemoryRepo.Notes.AddRange(notes);
        }
        catch (Exception ex)
        {
            HandleError($"Failed to refresh notes: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Refresh folders data
    /// </summary>
    public async Task RefreshFoldersAsync()
    {
        try
        {
            var foldersManager = _managerFactory.CreateFoldersManager();
            var folders = await foldersManager.GetAllSync();
            
            Folders.Clear();
            foreach (var folder in folders)
            {
                Folders.Add(folder);
            }

            // Update in-memory repo
            _inMemoryRepo.Folders.Clear();
            _inMemoryRepo.Folders.AddRange(folders);
        }
        catch (Exception ex)
        {
            HandleError($"Failed to refresh folders: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Refresh shortcuts data
    /// </summary>
    public async Task RefreshShortcutsAsync()
    {
        try
        {
            var shortcutsManager = _managerFactory.CreateShortcutsManager();
            var shortcuts = await shortcutsManager.GetAllSync();
            
            Shortcuts.Clear();
            foreach (var shortcut in shortcuts)
            {
                Shortcuts.Add(shortcut);
            }

            // Update in-memory repo
            _inMemoryRepo.Shortcuts.Clear();
            _inMemoryRepo.Shortcuts.AddRange(shortcuts);
        }
        catch (Exception ex)
        {
            HandleError($"Failed to refresh shortcuts: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Refresh tags data
    /// </summary>
    public Task RefreshTagsAsync()
    {
        try
        {
            // Assuming tags are managed through notes or separately
            // For now, we'll extract unique tags from notes
            var allTags = Notes.SelectMany(n => n.Tags).DistinctBy(t => t.Id).ToList();
            
            Tags.Clear();
            foreach (var tag in allTags)
            {
                Tags.Add(tag);
            }

            // Update in-memory repo
            _inMemoryRepo.Tags.Clear();
            _inMemoryRepo.Tags.AddRange(allTags);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            HandleError($"Failed to refresh tags: {ex.Message}", ex);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Refresh categories data
    /// </summary>
    public Task RefreshCategoriesAsync()
    {
        try
        {
            // Assuming categories are managed separately or through folders
            // For now, we'll create categories based on folder structure
            var allCategories = Folders.Select(f => new Category 
            { 
                Id = f.Id, 
                Name = f.Name
            }).ToList();
            
            Categories.Clear();
            foreach (var category in allCategories)
            {
                Categories.Add(category);
            }

            // Update in-memory repo
            _inMemoryRepo.Categories.Clear();
            _inMemoryRepo.Categories.AddRange(allCategories);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            HandleError($"Failed to refresh categories: {ex.Message}", ex);
            return Task.CompletedTask;
        }
    }

    // CRUD Operations with automatic UI updates

    public async Task<Note> AddNoteAsync(Note note)
    {
        try
        {
            var notesManager = _managerFactory.CreateNotesManager();
            var result = await notesManager.AddSync<Note>(note);
            
            Notes.Add(result);
            _inMemoryRepo.AddItem(result);
            
            OnDataChanged?.Invoke();
            return result;
        }
        catch (Exception ex)
        {
            HandleError($"Failed to add note: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<Note> UpdateNoteAsync(Note note)
    {
        try
        {
            var notesManager = _managerFactory.CreateNotesManager();
            var result = await notesManager.UpdateSync<Note>(note);
            
            var existingNote = Notes.FirstOrDefault(n => n.Id == note.Id);
            if (existingNote != null)
            {
                var index = Notes.IndexOf(existingNote);
                Notes[index] = result;
            }
            
            _inMemoryRepo.NotifyDataChanged();
            OnDataChanged?.Invoke();
            return result;
        }
        catch (Exception ex)
        {
            HandleError($"Failed to update note: {ex.Message}", ex);
            throw;
        }
    }

    public async Task DeleteNoteAsync(string noteId)
    {
        try
        {
            var notesManager = _managerFactory.CreateNotesManager();
            await notesManager.DeleteSync<object>(noteId);
            
            var noteToRemove = Notes.FirstOrDefault(n => n.Id == noteId);
            if (noteToRemove != null)
            {
                Notes.Remove(noteToRemove);
            }
            
            _inMemoryRepo.DeleteNotesByNotesId(noteId);
            OnDataChanged?.Invoke();
        }
        catch (Exception ex)
        {
            HandleError($"Failed to delete note: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<Folder> AddFolderAsync(Folder folder)
    {
        try
        {
            var foldersManager = _managerFactory.CreateFoldersManager();
            var result = await foldersManager.AddSync<Folder>(folder);
            
            Folders.Add(result);
            _inMemoryRepo.AddItem(result);
            
            OnDataChanged?.Invoke();
            return result;
        }
        catch (Exception ex)
        {
            HandleError($"Failed to add folder: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<Folder> UpdateFolderAsync(Folder folder)
    {
        try
        {
            var foldersManager = _managerFactory.CreateFoldersManager();
            var result = await foldersManager.UpdateSync<Folder>(folder);
            
            var existingFolder = Folders.FirstOrDefault(f => f.Id == folder.Id);
            if (existingFolder != null)
            {
                var index = Folders.IndexOf(existingFolder);
                Folders[index] = result;
            }
            
            _inMemoryRepo.NotifyDataChanged();
            OnDataChanged?.Invoke();
            return result;
        }
        catch (Exception ex)
        {
            HandleError($"Failed to update folder: {ex.Message}", ex);
            throw;
        }
    }

    public async Task DeleteFolderAsync(string folderId)
    {
        try
        {
            var foldersManager = _managerFactory.CreateFoldersManager();
            await foldersManager.DeleteSync<object>(folderId);
            
            var folderToRemove = Folders.FirstOrDefault(f => f.Id == folderId);
            if (folderToRemove != null)
            {
                Folders.Remove(folderToRemove);
            }
            
            _inMemoryRepo.DeleteFoldersByFolderId(folderId);
            OnDataChanged?.Invoke();
        }
        catch (Exception ex)
        {
            HandleError($"Failed to delete folder: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<Shortcut> AddShortcutAsync(Shortcut shortcut)
    {
        try
        {
            var shortcutsManager = _managerFactory.CreateShortcutsManager();
            var result = await shortcutsManager.AddSync<Shortcut>(shortcut);
            
            Shortcuts.Add(result);
            _inMemoryRepo.AddItem(result);
            
            OnDataChanged?.Invoke();
            return result;
        }
        catch (Exception ex)
        {
            HandleError($"Failed to add shortcut: {ex.Message}", ex);
            throw;
        }
    }

    public async Task<Shortcut> UpdateShortcutAsync(Shortcut shortcut)
    {
        try
        {
            var shortcutsManager = _managerFactory.CreateShortcutsManager();
            var result = await shortcutsManager.UpdateSync<Shortcut>(shortcut);
            
            var existingShortcut = Shortcuts.FirstOrDefault(s => s.Id == shortcut.Id);
            if (existingShortcut != null)
            {
                var index = Shortcuts.IndexOf(existingShortcut);
                Shortcuts[index] = result;
            }
            
            _inMemoryRepo.NotifyDataChanged();
            OnDataChanged?.Invoke();
            return result;
        }
        catch (Exception ex)
        {
            HandleError($"Failed to update shortcut: {ex.Message}", ex);
            throw;
        }
    }

    public async Task DeleteShortcutAsync(string shortcutId)
    {
        try
        {
            var shortcutsManager = _managerFactory.CreateShortcutsManager();
            await shortcutsManager.DeleteSync<object>(shortcutId);
            
            var shortcutToRemove = Shortcuts.FirstOrDefault(s => s.Id == shortcutId);
            if (shortcutToRemove != null)
            {
                Shortcuts.Remove(shortcutToRemove);
            }
            
            _inMemoryRepo.DeleteShortcutsByShortcutId(shortcutId);
            OnDataChanged?.Invoke();
        }
        catch (Exception ex)
        {
            HandleError($"Failed to delete shortcut: {ex.Message}", ex);
            throw;
        }
    }

    public Task<Tag> AddTagAsync(Tag tag)
    {
        try
        {
            Tags.Add(tag);
            _inMemoryRepo.AddItem(tag);
            
            OnDataChanged?.Invoke();
            return Task.FromResult(tag);
        }
        catch (Exception ex)
        {
            HandleError($"Failed to add tag: {ex.Message}", ex);
            throw;
        }
    }

    public Task<Tag> UpdateTagAsync(Tag tag)
    {
        try
        {
            var existingTag = Tags.FirstOrDefault(t => t.Id == tag.Id);
            if (existingTag != null)
            {
                var index = Tags.IndexOf(existingTag);
                Tags[index] = tag;
            }
            
            _inMemoryRepo.NotifyDataChanged();
            OnDataChanged?.Invoke();
            return Task.FromResult(tag);
        }
        catch (Exception ex)
        {
            HandleError($"Failed to update tag: {ex.Message}", ex);
            throw;
        }
    }

    public Task DeleteTagAsync(string tagId)
    {
        try
        {
            var tagToRemove = Tags.FirstOrDefault(t => t.Id == tagId);
            if (tagToRemove != null)
            {
                Tags.Remove(tagToRemove);
            }
            
            _inMemoryRepo.NotifyDataChanged();
            OnDataChanged?.Invoke();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            HandleError($"Failed to delete tag: {ex.Message}", ex);
            throw;
        }
    }

    public Task<Category> AddCategoryAsync(Category category)
    {
        try
        {
            Categories.Add(category);
            _inMemoryRepo.AddItem(category);
            
            OnDataChanged?.Invoke();
            return Task.FromResult(category);
        }
        catch (Exception ex)
        {
            HandleError($"Failed to add category: {ex.Message}", ex);
            throw;
        }
    }

    public Task<Category> UpdateCategoryAsync(Category category)
    {
        try
        {
            var existingCategory = Categories.FirstOrDefault(c => c.Id == category.Id);
            if (existingCategory != null)
            {
                var index = Categories.IndexOf(existingCategory);
                Categories[index] = category;
            }
            
            _inMemoryRepo.NotifyDataChanged();
            OnDataChanged?.Invoke();
            return Task.FromResult(category);
        }
        catch (Exception ex)
        {
            HandleError($"Failed to update category: {ex.Message}", ex);
            throw;
        }
    }

    public Task DeleteCategoryAsync(string categoryId)
    {
        try
        {
            var categoryToRemove = Categories.FirstOrDefault(c => c.Id == categoryId);
            if (categoryToRemove != null)
            {
                Categories.Remove(categoryToRemove);
            }
            
            _inMemoryRepo.NotifyDataChanged();
            OnDataChanged?.Invoke();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            HandleError($"Failed to delete category: {ex.Message}", ex);
            throw;
        }
    }

    // Query methods
    public List<Note> GetNotesByFolderId(string folderId) => 
        Notes.Where(n => n.FolderId == folderId).ToList();

    public List<Shortcut> GetShortcutsByFolderId(string folderId) => 
        Shortcuts.Where(s => s.FolderId == folderId).ToList();

    public List<Note> GetNotesByTagId(string tagId) => 
        Notes.Where(n => n.Tags.Any(t => t.Id == tagId)).ToList();

    public Note? GetNoteById(string id) => Notes.FirstOrDefault(n => n.Id == id);
    public Folder? GetFolderById(string id) => Folders.FirstOrDefault(f => f.Id == id);
    public Shortcut? GetShortcutById(string id) => Shortcuts.FirstOrDefault(s => s.Id == id);
    public Tag? GetTagById(string id) => Tags.FirstOrDefault(t => t.Id == id);
    public Category? GetCategoryById(string id) => Categories.FirstOrDefault(c => c.Id == id);

    // Private methods
    private void OnInMemoryDataChanged()
    {
        OnDataChanged?.Invoke();
    }

    private void HandleError(string message, Exception? exception = null)
    {
        LastError = message;
        OnError?.Invoke(message);
        // Log the error if you have a logging service
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        _inMemoryRepo.OnChange -= OnInMemoryDataChanged;
    }
}
