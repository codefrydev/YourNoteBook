namespace YourNoteBook.Shared.Models.DTOs.ResponseModels;

public class PopupDialogueOptionPayload
{
    public string Title { get; set; } = string.Empty;
    public object Options { get; set; } = new();
}