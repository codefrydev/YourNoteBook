using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using YourNoteBook.Infrastructure.Data.Context;
using YourNoteBook.Shared.Helpers;
using YourNoteBook.Core.Entities;
using YourNoteBook.Core.Interfaces;
using YourNoteBook.Shared.Utilities;
using YourNoteBook.Shared.Models.Results;

namespace YourNoteBook.Shared.Components.UI.AppBar;

public partial class FolderAppBar : ComponentBase
{
    
    [Inject] private IManager<Note> NotesManager { get; set; } = null!;
    [Inject] private IManager<Shortcut> ShortcutManager { get; set; } = null!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private IFirebaseHelper FirebaseHelper { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;
    [Inject] private IManager<Core.Entities.Folder> FolderManager { get; set; } = null!;
    bool _drawerOpen = true;
    public string Search { get; set; } = string.Empty;

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    async Task GoBack()
    {
        await JsRuntime.InvokeVoidAsync("history.back");
    }
    protected override void OnInitialized()
    {
        InMemoryRepo.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        InMemoryRepo.OnChange -= StateHasChanged;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (CurrentContext.IsAuthenticated)
        {
            await LoadAllData();
        }
    }

    private async Task LoadAllData()
    {
        if (!CurrentContext.IsAuthenticated)
            return;
        try
        {
            var notes = await NotesManager.GetAllSync();
            var shortcut = await ShortcutManager.GetAllSync();
            InMemoryRepo.Notes = notes.ToList();
            InMemoryRepo.Shortcuts = shortcut.ToList();
        }
        catch (Exception ex)
        {
            await JsRuntime.InvokeVoidAsync("alert", $"Error loading Data: {ex.Message}");
        }
    }

    private async Task AddNotes(MouseEventArgs arg)
    {
        var title = await JsRuntime.InvokeAsync<string>("prompt", "Enter note title:");
        if (!string.IsNullOrEmpty(title))
        {
            var content = await JsRuntime.InvokeAsync<string>("prompt", "Enter note content:");
            if (!string.IsNullOrEmpty(content) && CurrentContext.CurrentFolderId is not null)
            {
                var note = new Note
                {
                    Title = title,
                    Content = content,
                    FolderId = CurrentContext.CurrentFolderId,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                var response = await NotesManager.AddSync<SaveDocumentResult>(note);
                if (response.success)
                {
                    note.Id = response.id;
                    InMemoryRepo.AddItem(note);
                    await JsRuntime.InvokeVoidAsync("alert", "Note added successfully!");
                }
            }
        }
    }

    private async Task AddShortcut(MouseEventArgs arg)
    {
        var name = await JsRuntime.InvokeAsync<string>("prompt", "Enter shortcut name:");
        if (!string.IsNullOrEmpty(name))
        {
            var url = await JsRuntime.InvokeAsync<string>("prompt", "Enter URL:");
            if (!string.IsNullOrEmpty(url) && CurrentContext.CurrentFolderId is not null)
            {
                var shortcut = new Shortcut
                {
                    Action = name,
                    Keys = url,
                    FolderId = CurrentContext.CurrentFolderId,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                var response = await ShortcutManager.AddSync<SaveDocumentResult>(shortcut);
                if (response.success)
                {
                    shortcut.Id = response.id;
                    InMemoryRepo.AddItem(shortcut);
                    await JsRuntime.InvokeVoidAsync("alert", "Shortcut added successfully!");
                }
            }
        }

        StateHasChanged();
    }
}