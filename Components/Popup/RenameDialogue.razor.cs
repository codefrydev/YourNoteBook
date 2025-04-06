using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Models;

namespace YourNoteBook.Components.Popup;

public partial class RenameDialogue : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
    [Parameter] public FolderModel Model { get; set; } = null!;

    private void Submit() => MudDialog.Close(DialogResult.Ok(Model));

    private void Cancel() => MudDialog.Cancel();
}