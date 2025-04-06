using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Models;

namespace YourNoteBook.Components.Popup;

public partial class AddNotesDialogue : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; }

    public Note NewNote { get; set; } = new();
    private string _tags = string.Empty;

    private void Submit()
    {
        NewNote.Tags = _tags.Split([',','.',':'], StringSplitOptions.RemoveEmptyEntries)
            .Select(tag => new Tag
            { 
                Name = tag.Trim()
            })
            .ToList();
        MudDialog.Close(DialogResult.Ok(NewNote));
    }
    private void Cancel() => MudDialog.Cancel();
}