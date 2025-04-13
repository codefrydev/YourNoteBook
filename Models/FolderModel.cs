using System.Text.Json;
using MudBlazor;
using YourNoteBook.Models;
using YourNoteBook.Utils;

public class FolderModel : BaseModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

}