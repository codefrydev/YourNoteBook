using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using YourNoteBook.Data;
using YourNoteBook.Helper;
using YourNoteBook.Models;
using YourNoteBook.Services;
using YourNoteBook.Utils;

namespace YourNoteBook.Pages;

public partial class Home : ComponentBase
{
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!; 
    [Inject] private IFirebaseHelper FirebaseHelper { get; set; } = null!;
    [Inject] private IManager<FolderModel> FolderManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (CurrentContext.IsAuthenticated)
        {
            await LoadAllData();
        }
    }

    protected override void OnInitialized()
    {
        InMemoryRepo.OnChange += StateHasChanged;
    }

    public void Dispose()
    {
        InMemoryRepo.OnChange -= StateHasChanged;
    }
    private async Task LoadAllData()
    { 
        if(!CurrentContext.IsAuthenticated)
            return;
        try
        {
            var result = await FolderManager.GetAllSync();
            InMemoryRepo.Folders = result.ToList();
        }
        catch (Exception ex)
        {
            
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"Error loading contacts: {ex.Message}", Severity.Error);
        }
    }
}