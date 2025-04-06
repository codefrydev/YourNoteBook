using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using YourNoteBook.Components.Popup;

namespace YourNoteBook.Components;

public partial class ShortcutComponent : ComponentBase
{
    private Task Rename()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        return DialogService.ShowAsync<EditNotesDialogue>("Simple Dialog", options);
    }
    private Task Delete()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        return DialogService.ShowAsync<DeleteDialogue>("Delete Me", options);
    }
    private void HandleHover(MouseEventArgs e, bool isHovering)
    {
        var paper = new MudPaper
        {
            Style = isHovering ? "transform: scale(1.02); z-index: 1;" : "transform: scale(1); z-index: 0;"
        };
    }
}