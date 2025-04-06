using Microsoft.AspNetCore.Components;

namespace YourNoteBook.Pages;

public partial class Home : ComponentBase
{
    protected override void OnInitialized()
    {
        InMemoryRepo.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        InMemoryRepo.OnChange -= StateHasChanged;
    }
}