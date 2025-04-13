namespace YourNoteBook.Models;

public class BaseModel
{
    public string? Icon { get; set; } = "";
    public string? IconColor { get; set; } = "";
    public string? BackGroundColor { get; set; }="";
    public bool IsPinned { get; set; } = false;
}