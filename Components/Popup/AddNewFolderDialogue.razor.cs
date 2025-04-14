using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Models;

namespace YourNoteBook.Components.Popup;

public partial class AddNewFolderDialogue : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }
    private FolderModel Model { get; set; } = new(); 
    private void Submit()
    {
        Model.Created = DateTime.Now; 
        MudDialog.Close(DialogResult.Ok(Model)); 
    } 
    private void Cancel() => MudDialog.Cancel();
}