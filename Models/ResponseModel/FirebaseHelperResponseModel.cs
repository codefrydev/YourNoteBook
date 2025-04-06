using MudBlazor;

namespace YourNoteBook.Models;

public class FirebaseHelperResponseModel
{
    public string Message { get; set; } = string.Empty;

    public MudIcon Icon { get; set; } = new MudIcon();
    public bool Success { get; set; } = false;
}