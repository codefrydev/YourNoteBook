using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using YourNoteBook.Infrastructure.Data.Context;
using YourNoteBook.Shared.Utilities;

namespace YourNoteBook.Shared.Components.Layouts;

public partial class FolderNavMenu : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!; 
    private void GoToHomePage()
    { 
        NavigationManager.NavigateTo($"");
    }
    private void OpenFolder(string folderId)
    {
        if (string.IsNullOrEmpty(folderId))
            return;
        CurrentContext.CurrentFolderId = folderId;
        NavigationManager.NavigateTo($"folder/{folderId}");
    }
}