using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using YourNoteBook.Infrastructure.Data.Context;
using YourNoteBook.Core.Entities;
using YourNoteBook.Core.Interfaces;
using YourNoteBook.Shared.Utilities;
using YourNoteBook.Shared.Services.Utilities;

namespace YourNoteBook.Pages;

public partial class Folder : ComponentBase, IDisposable
{
    [Parameter] public string FolderId { get; set; } = string.Empty;
    private bool _isNoteShowing = true; // Default to showing notes
    private bool _isLoading = true;
    private bool _isDisposed = false;
    
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IManager<Note> NotesManager { get; set; } = null!;
    [Inject] private IManager<Shortcut> ShortcutManager { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;
    [Inject] private SnackbarService SnackbarService { get; set; } = null!;

    // Computed properties that react to InMemoryRepo changes
    public Core.Entities.Folder? CurrentFolder => InMemoryRepo.GetFolderById(FolderId);
    public List<Note> Notes => InMemoryRepo.GetNotesByFolderId(FolderId);
    public List<Shortcut> Shortcuts => InMemoryRepo.GetShortcutsByFolderId(FolderId);
    
    protected override async Task OnInitializedAsync()
    {
        if (!CurrentContext.IsAuthenticated)
        {
            NavigationManager.NavigateTo("");
            return;
        }
        
        // Set the current folder ID from the route parameter
        if (!string.IsNullOrEmpty(FolderId))
        {
            CurrentContext.CurrentFolderId = FolderId;
        }
        
        // Subscribe to data changes
        InMemoryRepo.OnChange += OnDataChanged;
        
        // Load data from Firebase into InMemoryRepo
        await LoadAllData();
        
        
        _isLoading = false;
        StateHasChanged();
        
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

    private void ShowShortcuts()
    {
        _isNoteShowing = false;
        StateHasChanged();
    }

    private void ShowNotes()
    {
        _isNoteShowing = true;
        StateHasChanged();
    }

    private void NavigateToHome()
    {
        NavigationManager.NavigateTo("");
    }

    private void OpenNote(string noteId)
    {
        // For now, we'll just show an alert. In a real app, you might navigate to a note editor
        SnackbarService.ShowInfo($"Opening note: {noteId}");
    }

    private void OpenShortcut(string shortcutId)
    {
        // For now, we'll just show an alert. In a real app, you might execute the shortcut
        SnackbarService.ShowInfo($"Executing shortcut: {shortcutId}");
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
                SnackbarService.ShowSuccess("Note created successfully!");
            }
            else
            {
                SnackbarService.ShowError("Failed to create note. Please try again.");
            }
        }
        catch (Exception ex)
        {
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

    private void AddShortcut()
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

    private async Task LoadAllData()
    {
        if (!CurrentContext.IsAuthenticated)
            return;
            
        try
        {
            _isLoading = true;
            StateHasChanged();
            
            // Load all notes and shortcuts from Firebase
            var allNotes = await NotesManager.GetAllSync();
            var allShortcuts = await ShortcutManager.GetAllSync();
            
            // Update InMemoryRepo - single source of truth
            InMemoryRepo.Notes = allNotes.ToList();
            InMemoryRepo.Shortcuts = allShortcuts.ToList();
            
            // Trigger change notification
            InMemoryRepo.NotifyDataChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowError($"Error loading data: {ex.Message}");
        }
        finally
        {
            _isLoading = false;
        }
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