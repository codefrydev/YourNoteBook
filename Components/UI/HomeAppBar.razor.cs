using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using YourNoteBook.Components.Popup;
using YourNoteBook.Data;
using YourNoteBook.Helper;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Components.UI;

public partial class HomeAppBar: ComponentBase
{
    private bool _drawerOpen = true;
    public string Search { get; set; } = string.Empty;
    [Parameter] public bool IsHamburgerMenuVisible { get; set; } = false;
    [Parameter] public string Heading { get; set; } = "\ud83d\udd25 App NoteBook";
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private IFirebaseHelper FirebaseHelper { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private IManager<FolderModel> FolderManager { get; set; } = null!;

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private async Task SyncFromLocalStorage()
    {
        var wasSuccess = await BlazoredLocalStorageHelper.RetrieveFromLocalStorage(LocalStorage);
        if (wasSuccess)
        {
            var firebaseModerResponse = await FirebaseHelper.ActivateFireBaseDb();
            if (firebaseModerResponse?.Success == true)
            {
                var result = await FolderManager.GetAllSync();
                InMemoryRepo.Folders = result.ToList(); 
            }
            else
            {
                Snackbar.Add(firebaseModerResponse?.Message ?? "Failed to sync data", Severity.Error);
            }
        }
    }

    private async Task AddNewFolder()
    {
        var options = PopUpDialogueStyle.GetDefaultDialogOptions();

        var dialog = await DialogService.ShowAsync<AddNewFolderDialogue>(options.Title, options.Options);
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: FolderModel })
        {
            if (CurrentContext.IsAuthenticated)
            {
                var folder = (FolderModel)result.Data;
                var response = await FolderManager.AddSync<SaveDocumentResult>(folder);
                if (response.success)
                {
                    folder.Id = response.id;
                    InMemoryRepo.AddItem(folder);
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                    Snackbar.Add($"Added {folder.Name}", Severity.Success);
                }
            }
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