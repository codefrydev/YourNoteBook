using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using YourNoteBook.Infrastructure.Data.Context;
using YourNoteBook.Shared.Helpers;
using YourNoteBook.Core.Entities;
using YourNoteBook.Core.Interfaces;
using YourNoteBook.Shared.Utilities;
using YourNoteBook.Shared.Services.Utilities;

namespace YourNoteBook.Pages;

public partial class Landing : ComponentBase
{
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    [Inject] private IFirebaseHelper FirebaseHelper { get; set; } = null!;
    [Inject] private IManager<Core.Entities.Folder> FolderManager { get; set; } = null!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private SnackbarService SnackbarService { get; set; } = null!;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        // Initialize theme from localStorage
        await InitializeTheme();
        
        var wasSuccess = await BlazoredLocalStorageHelper.RetrieveFromLocalStorage(LocalStorage);
        if (wasSuccess)
        {
            await FirebaseHelper.ActivateFireBaseDb();
        }

        if (CurrentContext.IsAuthenticated)
        {
            await LoadAllData();
            // Automatically redirect to Home page if already authenticated
            NavigationManager.NavigateTo("/Home");
        }
    }
    
    private async Task InitializeTheme()
    {
        try
        {
            // Get saved theme from localStorage
            var savedTheme = await JsRuntime.InvokeAsync<string>("localStorage.getItem", "theme");
            
            if (!string.IsNullOrEmpty(savedTheme))
            {
                CurrentContext.IsDarkMode = savedTheme == "dark";
            }
            else
            {
                // Check system preference if no saved theme
                var prefersDark = await JsRuntime.InvokeAsync<bool>("eval", "window.matchMedia('(prefers-color-scheme: dark)').matches");
                CurrentContext.IsDarkMode = prefersDark;
            }
            
            // Apply the theme to the DOM
            await JsRuntime.InvokeVoidAsync("eval", $"document.documentElement.classList.toggle('dark', {CurrentContext.IsDarkMode.ToString().ToLower()})");
        }
        catch (Exception)
        {
            // Fallback to light mode if there's an error
            CurrentContext.IsDarkMode = false;
            await JsRuntime.InvokeVoidAsync("eval", "document.documentElement.classList.remove('dark')");
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
            SnackbarService.ShowError($"Error loading contacts: {ex.Message}");
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
        _showFirebaseDialog = false;
    }

    private void OpenHomePage()
    {
        NavigationManager.NavigateTo("Home");
    }
    
    private async Task OpenThemeManager()
    {
        try
        {
            // Toggle the dark mode state
            CurrentContext.IsDarkMode = !CurrentContext.IsDarkMode;
            
            // Apply the theme change to the DOM
            await JsRuntime.InvokeVoidAsync("eval", $"document.documentElement.classList.toggle('dark', {CurrentContext.IsDarkMode.ToString().ToLower()})");
            
            // Save the theme preference to localStorage
            await JsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", CurrentContext.IsDarkMode ? "dark" : "light");
            
            // Show feedback to user
            var themeText = CurrentContext.IsDarkMode ? "Dark" : "Light";
            SnackbarService.ShowSuccess($"Switched to {themeText} theme!");
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            SnackbarService.ShowError($"Failed to toggle theme: {ex.Message}");
        }
    }
}