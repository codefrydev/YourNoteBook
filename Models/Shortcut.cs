namespace YourNoteBook.Models;

public class Shortcut
{
    public string Id { get; set; } = "";
    public string Action { get; set; } = "";
    public string Keys { get; set; } = "";
    public string Description { get; set; } = "";
    public Category Category { get; set; } = new Category();
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Modified { get; set; } = DateTime.Now;
    public string FolderId { get; set; } = "";
}