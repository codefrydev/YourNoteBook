using System.Text.Json;
using YourNoteBook.Utils;

public class FolderModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

}