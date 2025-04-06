using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace YourNoteBook.Components.Popup;

public partial class EditNotesDialogue : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    private void Submit() => MudDialog.Close(DialogResult.Ok(true));

    private void Cancel() => MudDialog.Cancel();
}