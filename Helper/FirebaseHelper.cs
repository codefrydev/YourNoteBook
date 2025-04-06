using System.Text.Json;
using System.Text.RegularExpressions;
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
}