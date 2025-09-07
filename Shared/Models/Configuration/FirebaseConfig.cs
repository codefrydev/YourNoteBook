using System.Text;

namespace YourNoteBook.Shared.Models.Configuration;

public class FirebaseConfig
{
    public static string ApiKey { get; set; } = "";
    public static string AuthDomain { get; set; } = "";
    public static string ProjectId { get; set; } = "";
    public static string StorageBucket { get; set; } = "";
    public static string MessagingSenderId { get; set; } = "";
    public static string AppId { get; set; } = "";

    public static string EncryptFirebaseConfig()
    {
        var data = Encoding.UTF8.GetBytes(
            $"{ApiKey}:{AuthDomain}:{ProjectId}:{StorageBucket}:{MessagingSenderId}:{AppId}");
        return Convert.ToBase64String(data);
    }

    public static object DecryptFirebaseConfig(string encryptedConfig)
    {
        var decryptedBytes = Convert.FromBase64String(encryptedConfig);
        var decryptedString = Encoding.UTF8.GetString(decryptedBytes);
        var configParts = decryptedString.Split(':');

        if (configParts.Length != 6) return decryptedString;
        ApiKey = configParts[0];
        AuthDomain = configParts[1];
        ProjectId = configParts[2];
        StorageBucket = configParts[3];
        MessagingSenderId = configParts[4];
        AppId = configParts[5];
        return new
        {
            apiKey = ApiKey,
            authDomain = AuthDomain,
            projectId = ProjectId,
            storageBucket = StorageBucket,
            messagingSenderId = MessagingSenderId,
            appId = AppId
        };
    }
}