using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using YourNoteBook.Helper;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Components.Popup;

public partial class FireabseDialogue : ComponentBase
{ 
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;
    private string _testResult=string.Empty;

    private string _configJson = string.Empty;
    private bool _testing; 
    private TestFireBaseState _testFireBaseState = new()
    {
        Icon = new MudIcon
        {
            Icon = Icons.Material.Filled.ScubaDiving,
            Color = Color.Primary,
        },
        Message = Constant.FirebaseConfigValidateAndTest,
    };
 

    private async Task TestFirebaseConfig()
    {
        _testing = true;
        CurrentContext.IsAuthenticated = false; 
        _testFireBaseState.Icon.Color = Color.Primary;
        _testFireBaseState.Icon.Icon = Icons.Material.Filled.ScubaDiving;
        _testFireBaseState.Message = Constant.FirebaseConfigTestInProgress; 
        try
        {  
            if (!string.IsNullOrWhiteSpace(_configJson))
            {
                var validate = FirebaseHelper.ValidateJson(_configJson);
                if (!validate.Success)
                { 
                    throw new Exception(validate.Message);
                }
            }
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
            _testResult = res ? Constant.FirebaseConfigInitiationSuccess
                : Constant.FirebaseConfigInitiationFailed;
             
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
                _testFireBaseState.Message = Constant.FirebaseConfigTestAgain;
                await JsRuntime.InvokeAsync<SaveDocumentResult>(Constant.DeleteDocument, [Constant.VerifyTest, result.id]);
                await BlazoredLocalStorageHelper.StoreInLocalStorage(LocalStorage, null);
            }
            else
            {
                CurrentContext.IsAuthenticated = true; 
                _testFireBaseState.Icon.Color = Color.Warning;
                _testFireBaseState.Icon.Icon = Icons.Material.Filled.Error;
                _testFireBaseState.Message = Constant.FirebaseConfigTestFailed;
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