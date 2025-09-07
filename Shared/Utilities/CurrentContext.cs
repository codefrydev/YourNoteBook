using YourNoteBook.Core.Entities;

namespace YourNoteBook.Shared.Utilities;

public static class CurrentContext
{
    public static string? CurrentFolderId { get; set; } 
    public static bool IsAuthenticated { get; set; }
    
    public static bool IsDarkMode { get; set; } = false;
}