using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using YourNoteBook.Infrastructure.Data.Context;
using YourNoteBook.Shared.Helpers;
using YourNoteBook.Core.Entities;
using YourNoteBook.Core.Interfaces;
using YourNoteBook.Shared.Utilities;

namespace YourNoteBook.Pages;

public partial class Landing : ComponentBase
{
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] private IFirebaseHelper FirebaseHelper { get; set; } = null!;
    [Inject] private IManager<Core.Entities.Folder> FolderManager { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
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
            InMemoryRepo.Folders = result.Cast<Core.Entities.Folder>().ToList();
        }
        catch (Exception ex)
        {
            await JsRuntime.InvokeVoidAsync("alert", $"Error loading contacts: {ex.Message}");
        }
    }

    private bool _showFirebaseDialog = false;

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
            await LoadAllData();
            await JsRuntime.InvokeVoidAsync("alert", "✅ Firebase connected successfully! You can now create folders and notes.");
        }
        else
        {
            await JsRuntime.InvokeVoidAsync("alert", "❌ Failed to connect to Firebase. Please check your configuration.");
        }
    }

    private void OnFirebaseDialogCancel()
    {
        _showFirebaseDialog = false;
    }

    private void OpenHomePage()
    {
        NavigationManager.NavigateTo("Home");
    }
    
    private async Task OpenThemeManager()
    {
        await JsRuntime.InvokeVoidAsync("alert", "Theme management is now handled by Tailwind CSS!");
        StateHasChanged();
    }
}