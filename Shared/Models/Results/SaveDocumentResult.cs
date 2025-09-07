namespace YourNoteBook.Shared.Models.Results;

public class SaveDocumentResult
{
    public string id { get; set; } = "";
    public bool success { get; set; }
    public string error { get; set; } = "";
}