using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using YourNoteBook.Helper;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Components.Popup;

public partial class FireBaseDialogue : ComponentBase
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

    protected override Task OnInitializedAsync()
    {
        if (CurrentContext.IsAuthenticated)
        {
            _testFireBaseState.Icon.Icon = Icons.Material.Filled.Check;
            _testFireBaseState.Icon.Color = Color.Success;
            _testFireBaseState.Message = Constant.FirebaseConfigInitiationSuccess;
        }
        return base.OnInitializedAsync();
    }

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
            var response = await FirebaseHelper.ActivateFireBaseDb(JsRuntime, LocalStorage);
            if (!response.Success)
            {
                throw new Exception(response.Message);
            }
            _testFireBaseState.Icon = response.Icon;
            _testFireBaseState.Message = response.Message;
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