namespace YourNoteBook.Models;

public class Folder
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.Now;
    public List<Shortcut> Shortcuts { get; set; } = new List<Shortcut>();
    public List<Note> Notes { get; set; } = new List<Note>();
}