using YourNoteBook.Shared.Models.DTOs.ResponseModels;

namespace YourNoteBook.Shared.Helpers;

public static class PopUpDialogueStyle
{
    public static PopupDialogueOptionPayload GetDefaultDialogOptions()
    {
        return new PopupDialogueOptionPayload
        {
            Title = "Default Title",
            Options = new { 
                CloseOnEscapeKey = true,
                BackgroundClass = "my-custom-class",
                CloseButton = true
            }
        }; 
    }
}