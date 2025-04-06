using System.Text.Json;
using Blazored.LocalStorage;
using YourNoteBook.Models;
using YourNoteBook.Utils;

namespace YourNoteBook.Helper;

public static class BlazoredLocalStorageHelper
{ 
    public static async Task<bool> RetrieveFromLocalStorage(ILocalStorageService localStorage)
    {
        try
        {
            var base64 = await localStorage.GetItemAsync<string>(Constant.BlazorLocalStorageFirebaseConfigName);
            if (string.IsNullOrEmpty(base64))
            {
                return false;
            }
            var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            var myDeserializedClass = JsonSerializer.Deserialize<FirebaseConfigFromJson>(json);
            if (myDeserializedClass != null)
            {
                FirebaseConfig.ApiKey = myDeserializedClass.apiKey;
                FirebaseConfig.AuthDomain = myDeserializedClass.authDomain;
                FirebaseConfig.ProjectId = myDeserializedClass.projectId;
                FirebaseConfig.StorageBucket = myDeserializedClass.storageBucket; 
                FirebaseConfig.MessagingSenderId = myDeserializedClass.messagingSenderId;
                FirebaseConfig.AppId = myDeserializedClass.appId;
                return true;
            }
            else
            {
                await localStorage.RemoveItemAsync(Constant.BlazorLocalStorageFirebaseConfigName);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e); 
        }
        
        return false;
    } 
    
    public static async Task StoreInLocalStorage(ILocalStorageService localStorage,FirebaseConfigFromJson? myDeserializedClass)
    {
        if (!CurrentContext.IsAuthenticated)
        {
            return;
        }
        myDeserializedClass ??= new FirebaseConfigFromJson()
        {
            apiKey = FirebaseConfig.ApiKey,
            authDomain = FirebaseConfig.AuthDomain,
            projectId = FirebaseConfig.ProjectId,
            storageBucket = FirebaseConfig.StorageBucket,
            messagingSenderId = FirebaseConfig.MessagingSenderId,
            appId = FirebaseConfig.AppId
        };
        var json = JsonSerializer.Serialize(myDeserializedClass, new JsonSerializerOptions { WriteIndented = true });
        var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));
        await localStorage.SetItemAsync(Constant.BlazorLocalStorageFirebaseConfigName, base64); 
    }
}