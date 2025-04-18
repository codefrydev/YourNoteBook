using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Models;

namespace YourNoteBook.Components.Popup;

public partial class AddShortcutDialogue : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    private Shortcut NewShortcut { get; set; } = new();
    string _selectedCategory = string.Empty;
    List<string> Categories { get; set; } = new()
    {
        "Selection",
        "Transform",
        "Modeling",
        "Editing",
        "Navigation",
        "Rendering",
        "Animation",
        "Other"
    };

    private void Submit()
    {
        // Set default category if none selected.
        if (string.IsNullOrEmpty(_selectedCategory))
        {
            _selectedCategory = Categories.Last();
        }
        NewShortcut.Category = new Category()
        {
            Name = _selectedCategory
        };
        MudDialog.Close(DialogResult.Ok(NewShortcut));
    }

    private void Cancel() => MudDialog.Cancel();
}