namespace YourNoteBook.Models;

public class FolderModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}