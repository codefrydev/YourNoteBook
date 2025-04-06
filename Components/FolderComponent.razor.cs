using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Components.Popup;
using YourNoteBook.Utils;

namespace YourNoteBook.Components;

public partial class FolderComponent : ComponentBase
{
    [Inject] NavigationManager NavigationManager { get; set; } = null!;
    [Parameter] public string FolderId { get; set; } = string.Empty;
    
    private Task Rename()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        return DialogService.ShowAsync<RenameDialogue>("Simple Dialog", options);
    }
    private Task Delete()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        return DialogService.ShowAsync<RenameDialogue>("Simple Dialog", options);
    } 
    private void OpenFolder()
    {
        if (string.IsNullOrEmpty(FolderId))
            return;
        CurrentContext.CurrentFolderId = FolderId;
        NavigationManager.NavigateTo($"folder");
    }
}