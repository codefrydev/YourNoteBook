using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using YourNoteBook.Infrastructure.Data.Context;
using YourNoteBook.Shared.Helpers;
using YourNoteBook.Core.Entities;
using YourNoteBook.Core.Interfaces;
using YourNoteBook.Shared.Utilities;
using YourNoteBook.Shared.Models.Results;
using YourNoteBook.Shared.Services.Utilities;

namespace YourNoteBook.Shared.Components.UI.AppBar;

public partial class HomeAppBar: ComponentBase
{
    private bool _drawerOpen = true;
    public string Search { get; set; } = string.Empty;
    [Parameter] public bool IsHamburgerMenuVisible { get; set; } = false;
    [Parameter] public string Heading { get; set; } = "\ud83d\udd25 App NoteBook";
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private IFirebaseHelper FirebaseHelper { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;
    [Inject] private IManager<Core.Entities.Folder> FolderManager { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private SnackbarService SnackbarService { get; set; } = null!;
 
    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private void ToggleDarkMode()
    {
        CurrentContext.IsDarkMode = !CurrentContext.IsDarkMode;
        // Toggle dark class on html element
        JsRuntime.InvokeVoidAsync("eval", $"document.documentElement.classList.toggle('dark', {CurrentContext.IsDarkMode.ToString().ToLower()})");
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
                // Show error message - you can implement a custom notification system
                SnackbarService.ShowError(firebaseModerResponse?.Message ?? "Failed to sync data");
            }
        }
    }

    private bool _showAddFolderDialog = false;

    private void AddNewFolder()
    {
        if (CurrentContext.IsAuthenticated)
        {
            _showAddFolderDialog = true;
        }
        else
        {
            SnackbarService.ShowWarning("Please connect to Firebase first!");
        }
    }

    private async Task OnFolderSaved(Folder folder)
    {
        _showAddFolderDialog = false;
        
        var response = await FolderManager.AddSync<SaveDocumentResult>(folder);
        if (response.success)
        {
            folder.Id = response.id;
            InMemoryRepo.AddItem(folder);
            SnackbarService.ShowSuccess($"Folder '{folder.Name}' created successfully!");
        }
        else
        {
            SnackbarService.ShowError("Failed to create folder. Please try again.");
        }
    }

    private void OnFolderDialogCancel()
    {
        _showAddFolderDialog = false;
    }

    private bool _showFirebaseDialog = false;

    protected override void OnInitialized()
    {
        // Show Firebase dialog by default if not authenticated
        if (!CurrentContext.IsAuthenticated)
        {
            _showFirebaseDialog = true;
            StateHasChanged();
        }
    }

    private void AddFirebaseCode()
    {
        _showFirebaseDialog = true;
    }

    private async Task OnFirebaseSaved(bool success)
    {
        _showFirebaseDialog = false;
        
        if (success)
        {
            CurrentContext.IsAuthenticated = true;
            SnackbarService.ShowSuccess("Firebase connected successfully! Redirecting to Home page...");
            // Automatically redirect to Home page
            NavigationManager.NavigateTo("/Home");
        }
        else
        {
            SnackbarService.ShowError("Failed to connect to Firebase. Please check your configuration.");
        }
    }

    private void OnFirebaseDialogCancel()
    {
        // Only allow closing if user is authenticated
        if (CurrentContext.IsAuthenticated)
        {
            _showFirebaseDialog = false;
        }
    }
}