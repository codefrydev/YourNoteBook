using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using YourNoteBook.Components.Popup;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Layout;

public partial class FolderLayout : LayoutComponentBase
{
    bool _drawerOpen = true; 
    public string Search { get; set; } = string.Empty;
    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    } 
    private async Task AddNotes(MouseEventArgs arg)
    {
        var options = new DialogOptions { 
            CloseOnEscapeKey = true ,
            BackgroundClass = "my-custom-class" ,
            CloseButton = true, 
        };

        var dialog = await DialogService.ShowAsync<AddNotesDialogue>("Add Notes", options);
        var result = await dialog.Result; 
        if (result is { Canceled: false, Data: Note } && CurrentContext.CurrentFolderId is not null)
        {
            var note = (Note)result.Data; 
            note.Id = Guid.NewGuid().ToString();
            note.FolderId = CurrentContext.CurrentFolderId;
            InMemoryRepo.AddItem(note);
        }
        StateHasChanged();
    }

    private async Task AddShortcut(MouseEventArgs arg)
    {
        var options = new DialogOptions { 
            CloseOnEscapeKey = true ,
            BackgroundClass = "my-custom-class" ,
            CloseButton = true, 
        };

        var dialog = await DialogService.ShowAsync<AddShortcutDialogue>("Add Shortcut", options);
        var result = await dialog.Result; 
        if (result is { Canceled: false, Data: Shortcut } && CurrentContext.CurrentFolderId is not null)
        {
            var shortcut = (Shortcut)result.Data; 
            shortcut.Id = Guid.NewGuid().ToString(); 
            shortcut.FolderId = CurrentContext.CurrentFolderId;
            InMemoryRepo.AddItem(shortcut);
        }
        StateHasChanged();
    } 
}