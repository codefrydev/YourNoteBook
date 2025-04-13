using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Data;
using YourNoteBook.Utils;

namespace YourNoteBook.Layout;

public partial class FolderNavMenu : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!; 
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