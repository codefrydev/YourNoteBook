namespace YourNoteBook.Shared.Models.Configuration;

public class FirebaseConfigFromJson
{
    public string apiKey { get; set; } = "";
    public string authDomain { get; set; } = "";
    public string projectId { get; set; } = "";
    public string storageBucket { get; set; } = "";
    public string messagingSenderId { get; set; } = "";
    public string appId { get; set; } = "";
    public string measurementId { get; set; } = "";
}