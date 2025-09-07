namespace YourNoteBook.Shared.Models.Configuration;

public class TestFireBaseState
{
    public string Icon { get; set; } = "🔥";
    public string Message { get; set; } = ""; 
    
    public class IconInfo
    {
        public string Icon { get; set; } = "🔥";
        public string Color { get; set; } = "default";
    }
    
    public IconInfo IconProperty { get; set; } = new IconInfo();
}