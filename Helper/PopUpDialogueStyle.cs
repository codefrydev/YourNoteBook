using MudBlazor;
using YourNoteBook.Models;

namespace YourNoteBook.Helper;

public static class PopUpDialogueStyle
{
    public static PopupDialogueOptionPayload GetDefaultDialogOptions()
    {
        return new PopupDialogueOptionPayload
        {
            Title = "Default Title",
            Options = new DialogOptions { 
                CloseOnEscapeKey = true ,
                BackgroundClass = "my-custom-class" ,
                CloseButton = true, 
            }
        }; 
    }
}