using MudBlazor;

namespace YourNoteBook.Models;

public class PopupDialogueOptionPayload
{
    public string Title { get; set; } = string.Empty;
    public DialogOptions Options { get; set; } = new();
}