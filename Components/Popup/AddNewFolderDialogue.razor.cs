using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Models;

namespace YourNoteBook.Components.Popup;

public partial class AddNewFolderDialogue : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }
    private Folder NewFolder { get; set; } = new Folder();

    private void Submit()
    {
        NewFolder.Created = DateTime.Now;
        NewFolder.Id = Guid.NewGuid().ToString();
        MudDialog.Close(DialogResult.Ok(NewFolder)); 
    } 

    private void Cancel() => MudDialog.Cancel();
}