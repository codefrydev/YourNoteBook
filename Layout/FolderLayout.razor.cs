using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using YourNoteBook.Components.Popup;
using YourNoteBook.Data;
using YourNoteBook.Helper;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Layout;

public partial class FolderLayout : LayoutComponentBase
{
    [Inject] private IManager<Note> NotesManager { get; set; } = null!;
    [Inject] private IManager<Shortcut> ShortcutManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
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
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"Error loading Data: {ex.Message}", Severity.Error);
        }
    }

    private async Task AddNotes(MouseEventArgs arg)
    {
        var options = PopUpDialogueStyle.GetDefaultDialogOptions();

        var dialog = await DialogService.ShowAsync<AddNotesDialogue>("Add Notes", options.Options);
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: Note } && CurrentContext.CurrentFolderId is not null)
        {
            var note = (Note)result.Data;

            note.FolderId = CurrentContext.CurrentFolderId;
            var response = await NotesManager.AddSync<SaveDocumentResult>(note);
            if (response.success)
            {
                note.Id = response.id;
                InMemoryRepo.AddItem(note);
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("Note added", Severity.Success);
            }
        }
    }

    private async Task AddShortcut(MouseEventArgs arg)
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            BackgroundClass = "my-custom-class",
            CloseButton = true,
        };

        var dialog = await DialogService.ShowAsync<AddShortcutDialogue>("Add Shortcut", options);
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: Shortcut } && CurrentContext.CurrentFolderId is not null)
        {
            var shortcut = (Shortcut)result.Data;
            shortcut.FolderId = CurrentContext.CurrentFolderId;
            var response = await ShortcutManager.AddSync<SaveDocumentResult>(shortcut);
            if (response.success)
            {
                shortcut.Id = response.id;
                InMemoryRepo.AddItem(shortcut);
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("ShortCut added", Severity.Success);
            }
        }

        StateHasChanged();
    }
}