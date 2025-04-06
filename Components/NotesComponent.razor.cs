using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Components.Popup;

namespace YourNoteBook.Components;

public partial class NotesComponent : ComponentBase
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
}