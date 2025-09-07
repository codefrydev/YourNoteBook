using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using YourNoteBook.Infrastructure.Data.Context;
using YourNoteBook.Core.Entities;
using YourNoteBook.Core.Interfaces;
using YourNoteBook.Shared.Utilities;
using YourNoteBook.Shared.Models.Configuration;

namespace YourNoteBook.Pages;

public partial class Home : ComponentBase, IDisposable
{
    private bool _isLoading = true;
    private bool _isDisposed = false;
    
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IManager<Note> NotesManager { get; set; } = null!;
    [Inject] private IManager<Shortcut> ShortcutManager { get; set; } = null!;
    [Inject] private IManager<Core.Entities.Folder> FolderManager { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;

    // Computed properties that react to InMemoryRepo changes
    public List<Core.Entities.Folder> Folders => InMemoryRepo.Folders;
    public List<Note> Notes => InMemoryRepo.Notes;
    public List<Shortcut> Shortcuts => InMemoryRepo.Shortcuts;
    public List<Tag> Tags => InMemoryRepo.Tags;

    protected override async Task OnInitializedAsync()
    {
        await JsRuntime.InvokeVoidAsync("console.log", $"Home page initialized. IsAuthenticated: {CurrentContext.IsAuthenticated}");
        
        if (!CurrentContext.IsAuthenticated)
        {
            await JsRuntime.InvokeVoidAsync("console.log", "Not authenticated, navigating to landing page");
            NavigationManager.NavigateTo("");
            return;
        }
        
        // Subscribe to data changes
        InMemoryRepo.OnChange += OnDataChanged;
        
        await JsRuntime.InvokeVoidAsync("console.log", "Starting data load from Home page");
        
        // Load data from Firebase into InMemoryRepo
        await LoadAllData();
        
        _isLoading = false;
        StateHasChanged();
        
        await JsRuntime.InvokeVoidAsync("console.log", "Home page initialization complete");
        
        await base.OnInitializedAsync();
    }

    private async void OnDataChanged()
    {
        if (_isDisposed) return;
        
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    /// <summary>
    /// Refreshes data from Firebase and updates UI
    /// </summary>
    public async Task RefreshDataAsync()
    {
        if (_isDisposed) return;
        
        await LoadAllData();
        StateHasChanged();
    }

    private async Task LoadAllData()
    {
        if (!CurrentContext.IsAuthenticated)
        {
            await JsRuntime.InvokeVoidAsync("console.log", "Not authenticated, skipping data load");
            return;
        }
            
        try
        {
            _isLoading = true;
            StateHasChanged();
            
            await JsRuntime.InvokeVoidAsync("console.log", "Starting to load data from Firebase");
            
            // Load all data from Firebase
            await JsRuntime.InvokeVoidAsync("console.log", "Loading folders...");
            var folders = await FolderManager.GetAllSync();
            await JsRuntime.InvokeVoidAsync("console.log", "Loading notes...");
            var notes = await NotesManager.GetAllSync();
            await JsRuntime.InvokeVoidAsync("console.log", "Loading shortcuts...");
            var shortcuts = await ShortcutManager.GetAllSync();
            
            await JsRuntime.InvokeVoidAsync("console.log", $"Loaded {folders.Count()} folders, {notes.Count()} notes, {shortcuts.Count()} shortcuts");
            
            // Update InMemoryRepo - single source of truth
            InMemoryRepo.Folders = folders.Cast<Core.Entities.Folder>().ToList();
            InMemoryRepo.Notes = notes.ToList();
            InMemoryRepo.Shortcuts = shortcuts.ToList();
            
            await JsRuntime.InvokeVoidAsync("console.log", $"InMemoryRepo updated: {InMemoryRepo.Folders.Count} folders, {InMemoryRepo.Notes.Count} notes, {InMemoryRepo.Shortcuts.Count} shortcuts");
            
            // Trigger change notification
            InMemoryRepo.NotifyDataChanged();
            
            await JsRuntime.InvokeVoidAsync("console.log", "Data change notification sent");
        }
        catch (Exception ex)
        {
            await JsRuntime.InvokeVoidAsync("alert", $"Error loading data: {ex.Message}");
            await JsRuntime.InvokeVoidAsync("console.log", $"Error: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private void NavigateToFolder(string folderId)
    {
        CurrentContext.CurrentFolderId = folderId;
        NavigationManager.NavigateTo($"/folder/{folderId}");
    }

    private void NavigateToNote(string noteId)
    {
        var note = Notes.FirstOrDefault(n => n.Id == noteId);
        if (note != null)
        {
            CurrentContext.CurrentFolderId = note.FolderId;
            NavigationManager.NavigateTo($"/folder/{note.FolderId}");
        }
    }

    private void NavigateToShortcut(string shortcutId)
    {
        var shortcut = Shortcuts.FirstOrDefault(s => s.Id == shortcutId);
        if (shortcut != null)
        {
            CurrentContext.CurrentFolderId = shortcut.FolderId;
            NavigationManager.NavigateTo($"/folder/{shortcut.FolderId}");
        }
    }

    private void NavigateToAllFolders()
    {
        NavigationManager.NavigateTo("/Folder");
    }

    private void ShowFolderOptions(string folderId)
    {
        JsRuntime.InvokeVoidAsync("showOptions", folderId, "Open Folder", "Rename", "Delete");
    }

    private async Task RenameFolder(string folderId)
    {
        var newName = await JsRuntime.InvokeAsync<string>("prompt", "Enter new folder name:");
        if (!string.IsNullOrEmpty(newName))
        {
            var folder = Folders.FirstOrDefault(f => f.Id == folderId);
            if (folder != null)
            {
                folder.Name = newName;
                await RefreshDataAsync();
            }
        }
    }

    private async Task DeleteFolder(string folderId)
    {
        var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this folder?");
        if (confirmed)
        {
            var folder = Folders.FirstOrDefault(f => f.Id == folderId);
            if (folder != null)
            {
                InMemoryRepo.Folders.Remove(folder);
                InMemoryRepo.NotifyDataChanged();
            }
        }
    }

    private void AddNewFolder()
    {
        NavigationManager.NavigateTo("/Folder");
    }

    private void AddFirebaseCode()
    {
        // Navigate to Firebase configuration or show Firebase setup dialog
        NavigationManager.NavigateTo("/");
    }

    private async Task CreateTestData()
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("console.log", "Creating test data...");
            
            // Create a test folder
            var testFolder = new Core.Entities.Folder
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Folder",
                Description = "This is a test folder created to verify data flow",
                Color = "#3B82F6",
                Created = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await JsRuntime.InvokeVoidAsync("console.log", "Saving test folder to Firebase...");
            var result = await FolderManager.AddSync<object>(testFolder);
            await JsRuntime.InvokeVoidAsync("console.log", $"Test folder saved: {result}");
            
            // Refresh data to show the new folder
            await RefreshDataAsync();
            
            await JsRuntime.InvokeVoidAsync("alert", "✅ Test folder created successfully! Check the UI to see it appear.");
        }
        catch (Exception ex)
        {
            await JsRuntime.InvokeVoidAsync("alert", $"❌ Error creating test data: {ex.Message}");
            await JsRuntime.InvokeVoidAsync("console.log", $"Error creating test data: {ex}");
        }
    }

    private async Task ListFirebaseCollections()
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("console.log", "Listing Firebase collections...");
            var collections = await JsRuntime.InvokeAsync<string[]>("listAllCollections");
            await JsRuntime.InvokeVoidAsync("console.log", $"Found collections: {string.Join(", ", collections)}");
            await JsRuntime.InvokeVoidAsync("alert", $"Firebase Collections: {string.Join(", ", collections)}");
        }
        catch (Exception ex)
        {
            await JsRuntime.InvokeVoidAsync("alert", $"❌ Error listing collections: {ex.Message}");
            await JsRuntime.InvokeVoidAsync("console.log", $"Error listing collections: {ex}");
        }
    }

    private async Task ShowFirebaseConfig()
    {
        try
        {
            var config = new
            {
                ProjectId = FirebaseConfig.ProjectId,
                AuthDomain = FirebaseConfig.AuthDomain,
                ApiKey = FirebaseConfig.ApiKey?.Substring(0, Math.Min(10, FirebaseConfig.ApiKey?.Length ?? 0)) + "...",
                StorageBucket = FirebaseConfig.StorageBucket,
                MessagingSenderId = FirebaseConfig.MessagingSenderId,
                AppId = FirebaseConfig.AppId
            };
            
            await JsRuntime.InvokeVoidAsync("console.log", "Firebase Configuration:", config);
            await JsRuntime.InvokeVoidAsync("alert", $"Firebase Config:\nProjectId: {config.ProjectId}\nAuthDomain: {config.AuthDomain}\nApiKey: {config.ApiKey}");
        }
        catch (Exception ex)
        {
            await JsRuntime.InvokeVoidAsync("alert", $"❌ Error showing config: {ex.Message}");
            await JsRuntime.InvokeVoidAsync("console.log", $"Error showing config: {ex}");
        }
    }

    private bool _showAddNoteDialog = false;

    private void CreateNote()
    {
        _showAddNoteDialog = true;
        StateHasChanged();
    }

    private async Task OnNoteSaved(Note note)
    {
        _showAddNoteDialog = false;
        
        try
        {
            // Save the note to Firebase
            var result = await NotesManager.AddSync<object>(note);
            
            if (result != null)
            {
                // Refresh data to show the new note
                await RefreshDataAsync();
                await JsRuntime.InvokeVoidAsync("alert", "✅ Note created successfully!");
            }
            else
            {
                await JsRuntime.InvokeVoidAsync("alert", "❌ Failed to create note. Please try again.");
            }
        }
        catch (Exception ex)
        {
            await JsRuntime.InvokeVoidAsync("alert", $"❌ Error creating note: {ex.Message}");
        }
        
        StateHasChanged();
    }

    private void OnNoteDialogCancel()
    {
        _showAddNoteDialog = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            InMemoryRepo.OnChange -= OnDataChanged;
            _isDisposed = true;
        }
    }
}