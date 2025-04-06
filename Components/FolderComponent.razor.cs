using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Components.Popup;
using YourNoteBook.Data;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Components;

public partial class FolderComponent : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IManager<FolderModel> FolderManager { get; set; } = null!;
    [EditorRequired][Parameter] public FolderModel Model { get; set; } = null!;
    
    private async Task Rename()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters<RenameDialogue>
        {
            { x => x.Model, Model }
        };
        var response = await DialogService.ShowAsync<RenameDialogue>("Simple Dialog",parameters, options);
        var result = await response.Result;
        if(result is { Canceled: false, Data: FolderModel })
        {
            var resp = await FolderManager.UpdateSync<SaveDocumentResult>(Model);
            if (resp.success)   
            {
                InMemoryRepo.Folders = InMemoryRepo.Folders.Where(x=>x.Id != Model.Id).ToList();
                InMemoryRepo.Folders.Add(Model);
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("Folder Renamed", Severity.Success);
            }
        }
    }
    private async Task Delete()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters<DeleteDialogue>
        {
            { x => x.NoteTitle, $"Do You want to Delete {Model.Name}" }
        };
        var response = await DialogService.ShowAsync<DeleteDialogue>("Simple Dialog", options);
        var result = await response.Result;
        if(result is { Canceled: false, Data: bool })
        {
            var resp = await FolderManager.DeleteSync<SaveDocumentResult>(Model.Id);
            if (resp.success)
            {
                InMemoryRepo.Folders = InMemoryRepo.Folders.Where(x => x.Id != Model.Id).ToList();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("Folder deleted", Severity.Warning);
                StateHasChanged();
            }
        }
    } 
    private void OpenFolder()
    {
        if (string.IsNullOrEmpty(Model.Id))
            return;
        CurrentContext.CurrentFolderId = Model.Id;
        NavigationManager.NavigateTo($"folder");
    }
}