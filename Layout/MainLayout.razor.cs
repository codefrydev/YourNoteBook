using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using YourNoteBook.Components.Popup;
using YourNoteBook.Helper;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Layout;

public partial class MainLayout : LayoutComponentBase
{
    private bool _drawerOpen = true; 
    public string Search { get; set; } = string.Empty;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private IFirebaseHelper IFirebaseHelper { get; set; } = null!;

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private async Task SyncFromLocalStorage()
    {
        var wasSuccess = await BlazoredLocalStorageHelper.RetrieveFromLocalStorage(LocalStorage);
        if (wasSuccess)
        {
            await IFirebaseHelper.ActivateFireBaseDb();
        }
    }
    private async Task AddNewFolder()
    {
        var options = PopUpDialogueStyle.GetDefaultDialogOptions();

        var dialog = await DialogService.ShowAsync<AddNewFolderDialogue>(options.Title, options.Options);
        var result = await dialog.Result; 
        if (result is { Canceled: false, Data: Folder })
        {
            var folder = (Folder)result.Data; 
            folder.Id = Guid.NewGuid().ToString();
            InMemoryRepo.AddItem(folder);
        }
        Console.WriteLine($"Folder Count {InMemoryRepo.Folders.Count}"); 
    }

    private async Task AddFirebaseCode()
    {
        var options = new DialogOptions { 
            CloseOnEscapeKey = true ,
            BackgroundClass = "my-custom-class" ,
            CloseButton = true, 
        };

        var dialog = await DialogService.ShowAsync<FireBaseDialogue>("Simple Dialog", options);
        var result = await dialog.Result; 
        if(result is { Canceled: false, Data: bool })
        {
            CurrentContext.IsAuthenticated = (bool)result.Data;
        }
    } 
}