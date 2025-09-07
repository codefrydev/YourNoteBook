using System.Text.Json;
using System.Text.RegularExpressions;
using Blazored.LocalStorage;
using Microsoft.JSInterop;
using YourNoteBook.Shared.Models.DTOs.ResponseModels;
using YourNoteBook.Infrastructure.Services.Firebase;
using YourNoteBook.Shared.Utilities;
using YourNoteBook.Shared.Models.Configuration;
using YourNoteBook.Shared.Models.Results;

namespace YourNoteBook.Shared.Helpers;

public interface IFirebaseHelper 
{
    FirebaseHelperResponseModel ValidateJson(string configJson);
    Task<FirebaseHelperResponseModel> ActivateFireBaseDb();
}
public class FirebaseHelper(ILocalStorageService localStorage,IFirebaseJsInteropService firebaseJsInteropService) : IFirebaseHelper
{ 
    public FirebaseHelperResponseModel ValidateJson(string configJson)
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
    
    public async Task<FirebaseHelperResponseModel> ActivateFireBaseDb()
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
        
        Console.WriteLine($"Firebase Config - ProjectId: {FirebaseConfig.ProjectId}");
        Console.WriteLine($"Firebase Config - AuthDomain: {FirebaseConfig.AuthDomain}");
        Console.WriteLine($"Firebase Config - ApiKey: {FirebaseConfig.ApiKey?.Substring(0, Math.Min(10, FirebaseConfig.ApiKey?.Length ?? 0))}...");
        if (!CurrentContext.IsAuthenticated)
        {
            var res= await firebaseJsInteropService.InitializeFirebaseAsync(config);
            response.Message = res ? Constant.FirebaseConfigInitiationSuccess
                : Constant.FirebaseConfigInitiationFailed;
        }
             
        var document = new 
        {
            name = "Test Document",
            description = "This is a test document"
        }; 
        var result = await firebaseJsInteropService.SaveAsync<SaveDocumentResult>(Constant.VerifyTest, document);

        if (result.success)
        {
            response.Success = true;
            CurrentContext.IsAuthenticated = true; 
            response.Icon = "✅";
            response.Message = Constant.FirebaseConfigTestAgain;
            await firebaseJsInteropService.DeleteAsync<SaveDocumentResult>(Constant.VerifyTest, result.id);
            await BlazoredLocalStorageHelper.StoreInLocalStorage(localStorage, null);
        }
        else
        {
            CurrentContext.IsAuthenticated = false; 
            response.Icon = "⚠️";
            response.Message = Constant.FirebaseConfigTestFailed;
        }
        return response;
    }
}