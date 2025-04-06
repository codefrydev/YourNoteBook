using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using YourNoteBook.Components.Popup;
using YourNoteBook.Data;
using YourNoteBook.Models;

namespace YourNoteBook.Components;

public partial class ShortcutComponent : ComponentBase
{
    [Inject] private IManager<Shortcut> ShortcutManager { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    
    [EditorRequired] [Parameter] public Shortcut Model { get; set; } = null!;
    
    private async Task EditShortCut()
    { 
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters<EditShortcutDialogue>
        { 
            { x => x.NewShortcut, Model }
        };
        var response = await DialogService.ShowAsync<EditShortcutDialogue>("Simple Dialog",parameters, options);
        var result  =  await response.Result;
        
        if(result is { Canceled: false, Data: Shortcut })
        { 
            var model = (Shortcut)result.Data;
            var resp = await ShortcutManager.UpdateSync<SaveDocumentResult>(model);
            if (resp.success)
            {
                InMemoryRepo.Shortcuts = InMemoryRepo.Shortcuts.Where(x => x.Id != model.Id).ToList();
                InMemoryRepo.Shortcuts.Add(model);
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("Shortcut Edited", Severity.Warning);
                StateHasChanged();
            }
        }
    }
    private async Task Delete()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        var response = await DialogService.ShowAsync<DeleteDialogue>("Delete Me", options);
        var result  =  await response.Result;
        
        if(result is { Canceled: false, Data: bool })
        { 
            var resp = await ShortcutManager.DeleteSync<SaveDocumentResult>(Model.Id);
            if (resp.success)
            {
                InMemoryRepo.Shortcuts = InMemoryRepo.Shortcuts.Where(x => x.Id != Model.Id).ToList();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("Shortcut deleted", Severity.Warning);
                StateHasChanged();
            }
        }
    }
    private void HandleHover(MouseEventArgs e, bool isHovering)
    {
        var paper = new MudPaper
        {
            Style = isHovering ? "transform: scale(1.02); z-index: 1;" : "transform: scale(1); z-index: 0;"
        };
    }
}