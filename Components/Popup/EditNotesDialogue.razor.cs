using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Models;

namespace YourNoteBook.Components.Popup;

public partial class EditNotesDialogue : ComponentBase
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } 
    [Parameter] public Note Model { get; set; } = new();
    private string _tags = string.Empty;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _tags = string.Join(", ", Model.Tags.Select(tag => tag.Name)); 
    }

    private void Submit()
    {
        Model.Tags = _tags.Split([',','.',':'], StringSplitOptions.RemoveEmptyEntries)
            .Select(tag => new Tag
            { 
                Id = Guid.NewGuid().ToString(),
                Name = tag.Trim()
            })
            .ToList();
        MudDialog.Close(DialogResult.Ok(Model));
    }

    private void Cancel() => MudDialog.Cancel();
}