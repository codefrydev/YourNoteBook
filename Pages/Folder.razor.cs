using Microsoft.AspNetCore.Components;
using YourNoteBook.Utils;

namespace YourNoteBook.Pages;

public partial class Folder : ComponentBase
{
    [Parameter] public string FolderId { get; set; } = string.Empty;
    private bool _isNoteShowing;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    protected override Task OnInitializedAsync()
    {
        if (!CurrentContext.IsAuthenticated)
        {
            NavigationManager.NavigateTo("");
        }
        return base.OnInitializedAsync();
    }

    void ShowShortCut()
    {
        _isNoteShowing = false;
    }

    void ShowNotes()
    {
        _isNoteShowing = true;
    }

    protected override void OnInitialized()
    {
        InMemoryRepo.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        InMemoryRepo.OnChange -= StateHasChanged;
    }
}