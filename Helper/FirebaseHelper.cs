using System.Text.Json;
using System.Text.RegularExpressions;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using MudBlazor;
using YourNoteBook.Components.Popup;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Helper;

public class FirebaseHelper
{ 
    public static FirebaseHelperResponseModel ValidateJson(string configJson)
    {
        var response = new FirebaseHelperResponseModel();
        if (string.IsNullOrWhiteSpace(configJson))
        {
            response.Message = "Please enter a valid JSON string."; 
            return response;
        }

        try
        {
            var regex = new Regex(Constant.FirebaseConfigPattern, RegexOptions.Multiline);
            var data =  regex.Matches(configJson)  
                .Select(m => $"""
                                  "{m.Groups["key"].Value}" : "{m.Groups["value"].Value}"
                              """);

            var finalData = "{ \n" + string.Join(",\n", data)+ "\n}";
            var myDeserializedClass = JsonSerializer.Deserialize<FirebaseConfigFromJson>(finalData);
            if (myDeserializedClass != null)
            {
                FirebaseConfig.ApiKey = myDeserializedClass.apiKey;
                FirebaseConfig.AuthDomain = myDeserializedClass.authDomain;
                FirebaseConfig.ProjectId = myDeserializedClass.projectId;
                FirebaseConfig.StorageBucket = myDeserializedClass.storageBucket; 
                FirebaseConfig.MessagingSenderId = myDeserializedClass.messagingSenderId;
                FirebaseConfig.AppId = myDeserializedClass.appId;
                response.Success = true; 
            }
            else
            {
                response.Message= "Failed to deserialize JSON.";  
            }

        }
        catch (Exception e)
        {
            response.Message = e.Message;
        } 
        return response;
    }
    
    public static async Task<FirebaseHelperResponseModel> ActivateFireBaseDb(IJSRuntime jsRuntime, ILocalStorageService localStorage)
    {
        var response = new FirebaseHelperResponseModel();
        var config = new
        {
            apiKey = FirebaseConfig.ApiKey,
            authDomain = FirebaseConfig.AuthDomain,
            projectId = FirebaseConfig.ProjectId,
            storageBucket = FirebaseConfig.StorageBucket,
            messagingSenderId = FirebaseConfig.MessagingSenderId,
            appId = FirebaseConfig.AppId
        };
        if (!CurrentContext.IsAuthenticated)
        {
            var res= await jsRuntime.InvokeAsync<bool>(Constant.InitializeFirebase, config);
            response.Message = res ? Constant.FirebaseConfigInitiationSuccess
                : Constant.FirebaseConfigInitiationFailed;
        }
             
        var document = new 
        {
            name = "Test Document",
            description = "This is a test document"
        }; 
        var result = await jsRuntime.InvokeAsync<SaveDocumentResult>(Constant.SaveDocument, [Constant.VerifyTest, document]);

        if (result.success)
        {
            response.Success = true;
            CurrentContext.IsAuthenticated = true; 
            response.Icon.Color = Color.Success;
            response.Icon.Icon = Icons.Material.Filled.Check;
            response.Message = Constant.FirebaseConfigTestAgain;
            await jsRuntime.InvokeAsync<SaveDocumentResult>(Constant.DeleteDocument, [Constant.VerifyTest, result.id]);
            await BlazoredLocalStorageHelper.StoreInLocalStorage(localStorage, null);
        }
        else
        {
            CurrentContext.IsAuthenticated = false; 
            response.Icon.Color = Color.Warning;
            response.Icon.Icon = Icons.Material.Filled.Error;
            response.Message = Constant.FirebaseConfigTestFailed;
        }
        return response;
    }
}