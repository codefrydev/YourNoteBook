using MudBlazor;
using YourNoteBook.Models;

namespace YourNoteBook.Utils;

public static class CurrentContext
{
    public static string? CurrentFolderId { get; set; } 
    public static bool IsAuthenticated { get; set; }
    
    public static bool IsDarkMode { get; set; } = false;
    public static MudTheme Theme { get; set; } = ThemeChooser.Ceruleantheme;
}