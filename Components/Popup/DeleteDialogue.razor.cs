using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace YourNoteBook.Components.Popup;

public partial class DeleteDialogue : ComponentBase
{
    
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } 
    [Parameter]
    public string NoteTitle { get; set; }

    private void ConfirmDelete() => MudDialog.Close(DialogResult.Ok(true)); 
 

    private void Cancel() => MudDialog.Cancel();
}