using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using YourNoteBook.Infrastructure.Data.Context;
using YourNoteBook.Core.Entities;
using YourNoteBook.Core.Interfaces;
using YourNoteBook.Shared.Utilities;
using YourNoteBook.Shared.Models.Configuration;
using YourNoteBook.Shared.Services.Utilities;
using YourNoteBook.Shared.Services.SEO;
using YourNoteBook.Shared.Models.SEO;

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
    [Inject] private SnackbarService SnackbarService { get; set; } = null!;
    [Inject] private ISeoMetadataService SeoService { get; set; } = null!;

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
        
        // Set SEO metadata for home page
        await SetSeoMetadataAsync();
        
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
            SnackbarService.ShowError($"Error loading data: {ex.Message}");
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
        NavigationManager.NavigateTo($"folder/{folderId}");
    }

    private void NavigateToNote(string noteId)
    {
        var note = Notes.FirstOrDefault(n => n.Id == noteId);
        if (note != null)
        {
            CurrentContext.CurrentFolderId = note.FolderId;
            NavigationManager.NavigateTo($"folder/{note.FolderId}");
        }
    }

    private void NavigateToShortcut(string shortcutId)
    {
        var shortcut = Shortcuts.FirstOrDefault(s => s.Id == shortcutId);
        if (shortcut != null)
        {
            CurrentContext.CurrentFolderId = shortcut.FolderId;
            NavigationManager.NavigateTo($"folder/{shortcut.FolderId}");
        }
    }

    private void NavigateToAllFolders()
    {
        NavigationManager.NavigateTo("Folder");
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
        NavigationManager.NavigateTo("Folder");
    }

    private void AddFirebaseCode()
    {
        // Navigate to Firebase configuration or show Firebase setup dialog
        NavigationManager.NavigateTo("");
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
            
            SnackbarService.ShowSuccess("Test folder created successfully! Check the UI to see it appear.");
        }
        catch (Exception ex)
        {
            SnackbarService.ShowError($"Error creating test data: {ex.Message}");
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
            SnackbarService.ShowInfo($"Firebase Collections: {string.Join(", ", collections)}");
        }
        catch (Exception ex)
        {
            SnackbarService.ShowError($"Error listing collections: {ex.Message}");
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
            SnackbarService.ShowInfo($"Firebase Config: ProjectId: {config.ProjectId}, AuthDomain: {config.AuthDomain}, ApiKey: {config.ApiKey}");
        }
        catch (Exception ex)
        {
            SnackbarService.ShowError($"Error showing config: {ex.Message}");
            await JsRuntime.InvokeVoidAsync("console.log", $"Error showing config: {ex}");
        }
    }

    private bool _showAddNoteDialog = false;

    private void CreateNote()
    {
        Console.WriteLine("[Home] CreateNote called");
        _showAddNoteDialog = true;
        StateHasChanged();
        Console.WriteLine("[Home] CreateNote - dialog should be visible now");
    }

    private async Task OnNoteSaved(Note note)
    {
        Console.WriteLine("[Home] OnNoteSaved called");
        _showAddNoteDialog = false;
        
        try
        {
            Console.WriteLine("[Home] Saving note to Firebase...");
            // Save the note to Firebase
            var result = await NotesManager.AddSync<object>(note);
            Console.WriteLine($"[Home] Firebase save result: {result}");
            
            if (result != null)
            {
                Console.WriteLine("[Home] Note saved successfully, refreshing data...");
                // Refresh data to show the new note
                await RefreshDataAsync();
                Console.WriteLine("[Home] Calling SnackbarService.ShowSuccess...");
                SnackbarService.ShowSuccess("Note created successfully!");
                Console.WriteLine("[Home] SnackbarService.ShowSuccess called");
            }
            else
            {
                Console.WriteLine("[Home] Note save failed, showing error snackbar...");
                SnackbarService.ShowError("Failed to create note. Please try again.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Home] Exception in OnNoteSaved: {ex.Message}");
            SnackbarService.ShowError($"Error creating note: {ex.Message}");
        }
        
        StateHasChanged();
    }

    private void OnNoteDialogCancel()
    {
        _showAddNoteDialog = false;
        StateHasChanged();
    }

    private bool _showAddShortcutDialog = false;

    private void CreateShortcut()
    {
        _showAddShortcutDialog = true;
        StateHasChanged();
    }

    private async Task OnShortcutSaved(Shortcut shortcut)
    {
        _showAddShortcutDialog = false;
        
        try
        {
            // Save the shortcut to Firebase
            var result = await ShortcutManager.AddSync<object>(shortcut);
            
            if (result != null)
            {
                // Refresh data to show the new shortcut
                await RefreshDataAsync();
                SnackbarService.ShowSuccess("Shortcut created successfully!");
            }
            else
            {
                SnackbarService.ShowError("Failed to create shortcut. Please try again.");
            }
        }
        catch (Exception ex)
        {
            SnackbarService.ShowError($"Error creating shortcut: {ex.Message}");
        }
        
        StateHasChanged();
    }

    private void OnShortcutDialogCancel()
    {
        _showAddShortcutDialog = false;
        StateHasChanged();
    }

    private void TestSnackbar()
    {
        Console.WriteLine("[Home] TestSnackbar called");
        SnackbarService.ShowSuccess("Test snackbar message!");
        Console.WriteLine("[Home] TestSnackbar - ShowSuccess called");
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            InMemoryRepo.OnChange -= OnDataChanged;
            _isDisposed = true;
        }
    }
    
    private async Task SetSeoMetadataAsync()
    {
        try
        {
            var folderCount = Folders.Count;
            var noteCount = Notes.Count;
            var shortcutCount = Shortcuts.Count;
            
            // Dynamic title and description based on content
            var title = $"YourNoteBook Dashboard - {folderCount} Folders, {noteCount} Notes";
            var description = $"Manage your digital workspace with {folderCount} folders, {noteCount} notes, and {shortcutCount} shortcuts. Organize your thoughts and boost productivity.";
            
            // Set basic page metadata
            await SeoService.SetPageMetadataAsync(
                title: title,
                description: description,
                keywords: "dashboard, notes, folders, productivity, organization, digital workspace",
                imageUrl: "https://yournotebook.com/icon-192.png",
                url: "https://yournotebook.com/home"
            );

            // Set Open Graph metadata
            await SeoService.SetOpenGraphAsync(
                title: title,
                description: description,
                imageUrl: "https://yournotebook.com/icon-192.png",
                url: "https://yournotebook.com/home"
            );

            // Set Twitter Card metadata
            await SeoService.SetTwitterCardAsync(
                title: title,
                description: description,
                imageUrl: "https://yournotebook.com/icon-192.png",
                url: "https://yournotebook.com/home"
            );

            // Set JSON-LD structured data for dashboard
            var webPageJsonLd = new WebPageJsonLd
            {
                Name = title,
                Description = description,
                Url = "https://yournotebook.com/home",
                DatePublished = DateTime.Now.ToString("yyyy-MM-dd"),
                DateModified = DateTime.Now.ToString("yyyy-MM-dd")
            };

            // Create breadcrumb for navigation
            var breadcrumbJsonLd = new BreadcrumbListJsonLd
            {
                ItemListElement = new List<BreadcrumbItemJsonLd>
                {
                    new() { Position = 1, Name = "Home", Item = "https://yournotebook.com" },
                    new() { Position = 2, Name = "Dashboard", Item = "https://yournotebook.com/home" }
                }
            };

            await SeoService.SetJsonLdAsync(webPageJsonLd);
        }
        catch (Exception ex)
        {
            // Log error but don't break the page
            Console.WriteLine($"Error setting SEO metadata: {ex.Message}");
        }
    }
}