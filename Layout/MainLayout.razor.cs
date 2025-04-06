using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Components.Popup;
using YourNoteBook.Helper;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Layout;

public partial class MainLayout : LayoutComponentBase
{
    bool _drawerOpen = true; 
    public string Search { get; set; } = string.Empty;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private async Task SyncFromLocalStorage()
    {
        await BlazoredLocalStorageHelper.RetriveFromLocalStorage(LocalStorage);
    }
    private async Task AddNewFolder()
    {
        var options = new DialogOptions { 
            CloseOnEscapeKey = true ,
            BackgroundClass = "my-custom-class" ,
            CloseButton = true, 
        };

        var dialog = await DialogService.ShowAsync<AddNewFolderDialogue>("Simple Dialog", options);
        var result = await dialog.Result; 
        if (result is { Canceled: false, Data: Folder })
        {
            var folder = (Folder)result.Data; 
            folder.Id = Guid.NewGuid().ToString();
            InMemoryRepo.AddItem<Folder>(folder);
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

        var dialog = await DialogService.ShowAsync<FireabseDialogue>("Simple Dialog", options);
        var result = await dialog.Result; 
        if(result is { Canceled: false, Data: bool })
        {
            CurrentContext.IsAuthenticated = (bool)result.Data;
        }
    } 
}