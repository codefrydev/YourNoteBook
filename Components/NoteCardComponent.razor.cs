using Microsoft.AspNetCore.Components;
using MudBlazor;
using YourNoteBook.Components.Popup;
using YourNoteBook.Data;
using YourNoteBook.Models;

namespace YourNoteBook.Components;

public partial class NoteCardComponent : ComponentBase
{
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IManager<Note> Manager { get; set; } = null!;
    [Inject] private InMemoryRepo InMemoryRepo { get; set; } = null!;
    [EditorRequired][Parameter] public Note Model { get; set; } = new();
    
    private async Task EditNote()
    { 
        var options = new DialogOptions { CloseOnEscapeKey = true };
        var parameters = new DialogParameters<EditNotesDialogue>
        { 
            { x => x.NewNote, Model }
        };
        var response = await DialogService.ShowAsync<EditNotesDialogue>("Simple Dialog",parameters, options);
        var result  =  await response.Result;
        
        if(result is { Canceled: false, Data: Note })
        { 
            var model = (Note)result.Data;
            var resp = await Manager.UpdateSync<SaveDocumentResult>(model);
            if (resp.success)
            {
                model.Id = resp.id;
                InMemoryRepo.Notes = InMemoryRepo.Notes.Where(x => x.Id != model.Id).ToList();
                InMemoryRepo.Notes.Add(model);
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("Notes Edited", Severity.Warning);
                StateHasChanged();
            }
        }
    }
    private async Task Delete()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };
 
        var response = await DialogService.ShowAsync<DeleteDialogue>("Delete Me", options);
        var result  =  await response.Result;
        
        if(result is { Canceled: false, Data: bool })
        {  
            var resp = await Manager.DeleteSync<SaveDocumentResult>(Model.Id);
            if (resp.success)
            { 
                InMemoryRepo.Notes = InMemoryRepo.Notes.Where(x => x.Id != Model.Id).ToList(); 
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
                Snackbar.Add("Notes deleted", Severity.Warning);
                StateHasChanged();
            }
        }
    } 
}