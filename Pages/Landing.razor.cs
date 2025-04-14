using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Components.Popup;
using YourNoteBook.Data;
using YourNoteBook.Helper;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Pages;

public partial class Landing : ComponentBase
{
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] private IFirebaseHelper FirebaseHelper { get; set; } = null!;
    [Inject] private IManager<FolderModel> FolderManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var wasSuccess = await BlazoredLocalStorageHelper.RetrieveFromLocalStorage(LocalStorage);
        if (wasSuccess)
        {
            await FirebaseHelper.ActivateFireBaseDb();
        }

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
        if (!CurrentContext.IsAuthenticated)
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

    private async Task AddFirebaseCode()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            BackgroundClass = "my-custom-class",
            CloseButton = true,
        };
        var dialog = await DialogService.ShowAsync<FireBaseDialogue>("Simple Dialog", options);
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: bool })
        {
            CurrentContext.IsAuthenticated = (bool)result.Data;
        }
    }
}