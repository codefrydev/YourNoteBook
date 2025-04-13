using System.Reflection;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Models; 
namespace YourNoteBook.Components.Popup;

public partial class AddNewFolderDialogue : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }
    private FolderModel NewFolder { get; set; } = new();

    private List<string> IconsList { get;} =
    [
        "fas fa-atom",
        "fas fa-folder-open",
        "fas fa-folder-plus",
        "fas fa-folder-minus",
    ]; 

    private void Submit()
    {
        NewFolder.Created = DateTime.Now; 
        MudDialog.Close(DialogResult.Ok(NewFolder)); 
    } 
    private void Cancel() => MudDialog.Cancel();
}