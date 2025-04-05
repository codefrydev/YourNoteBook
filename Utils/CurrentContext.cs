using YourNoteBook.Models;

namespace YourNoteBook.Utils;

public class CurrentContext
{
    public static string? CurrentFolderId { get; set; }
    public static FirebaseConfig? FirebaseConfig { get; set; }
}