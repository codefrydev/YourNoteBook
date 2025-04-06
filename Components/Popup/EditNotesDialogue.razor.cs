using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Models;

namespace YourNoteBook.Components.Popup;

public partial class EditNotesDialogue : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } 
    [Parameter] public Note NewNote { get; set; } = new();
    private string _tags = string.Empty;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _tags = string.Join(", ", NewNote.Tags.Select(tag => tag.Name)); 
    }

    private void Submit()
    {
        NewNote.Tags = _tags.Split([',','.',':'], StringSplitOptions.RemoveEmptyEntries)
            .Select(tag => new Tag
            { 
                Id = Guid.NewGuid().ToString(),
                Name = tag.Trim()
            })
            .ToList();
        MudDialog.Close(DialogResult.Ok(NewNote));
    }

    private void Cancel() => MudDialog.Cancel();
}