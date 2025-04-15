using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Models;

namespace YourNoteBook.Components.Popup;

public partial class EditShortcutDialogue : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter] public Shortcut Model { get; set; } = null!;
    
    string _selectedCategory = string.Empty;
    List<string> Categories { get; } =
    [
        "Selection",
        "Transform",
        "Modeling",
        "Editing",
        "Navigation",
        "Rendering",
        "Animation",
        "Other"
    ];

    private void Submit()
    {
        // Set default category if none selected.
        if (string.IsNullOrEmpty(_selectedCategory))
        {
            _selectedCategory = Categories.Last();
        }
        Model.Category = new Category()
        {
            Name = _selectedCategory
        };
        MudDialog.Close(DialogResult.Ok(Model));
    }

    private void Cancel() => MudDialog.Cancel();
}