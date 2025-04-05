using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Components.Popup;

public partial class FireabseDialogue : ComponentBase
{ 
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!; 
    private string _testResult=string.Empty; 
    
    private bool _testing = false; 
    private TestFireBaseState _testFireBaseState = new()
    {
        Icon = new MudIcon
        {
            Icon = Icons.Material.Filled.ScubaDiving,
            Color = Color.Primary,
        },
        Message = "Test Firebase Connection",
    };
    private async Task TestFirebaseConfig()
    {
        _testing = true;
        CurrentContext.IsAuthenticated = false; 
        _testFireBaseState.Icon.Color = Color.Primary;
        _testFireBaseState.Icon.Icon = Icons.Material.Filled.ScubaDiving;
        _testFireBaseState.Message = "Testing in Progress...";
        try
        {  
            var config = new
            {
                apiKey = FirebaseConfig.ApiKey,
                authDomain = FirebaseConfig.AuthDomain,
                projectId = FirebaseConfig.ProjectId,
                storageBucket = FirebaseConfig.StorageBucket,
                messagingSenderId = FirebaseConfig.MessagingSenderId,
                appId = FirebaseConfig.AppId
            };
            var res= await JsRuntime.InvokeAsync<bool>(Constant.InitializeFirebase, config);
            _testResult = res ? "Firebase initialized successfully." : "Failed to initialize Firebase.";
             
            var document = new 
            {
                name = "Test Document",
                description = "This is a test document"
            }; 
            var result = await JsRuntime.InvokeAsync<SaveDocumentResult>(Constant.SaveDocument, [Constant.VerifyTest, document]);

            if (result.success)
            {
                CurrentContext.IsAuthenticated = true; 
                _testFireBaseState.Icon.Color = Color.Success;
                _testFireBaseState.Icon.Icon = Icons.Material.Filled.Check;
                _testFireBaseState.Message = "Test Again";
                await JsRuntime.InvokeAsync<SaveDocumentResult>(Constant.DeleteDocument, [Constant.VerifyTest, result.id]);
            }
            else
            {
                CurrentContext.IsAuthenticated = true; 
                _testFireBaseState.Icon.Color = Color.Warning;
                _testFireBaseState.Icon.Icon = Icons.Material.Filled.Error;
                _testFireBaseState.Message = "Test Failed";
            }
        }
        catch (Exception ex)
        {
            _testResult = $"Error invoking test: {ex.Message}";
        }
        _testing = false;
    }

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;
     private void CloseDialogue() => MudDialog.Close(DialogResult.Ok(true));   
}