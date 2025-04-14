namespace YourNoteBook.Utils;

public static class Constant
{
    public const string InitializeFirebase = "initializeFirebase"; 
    public const string SaveDocument = "saveDocument"; 
    public const string GetDocument = "getDocument"; 
    public const string GetAllDocument = "getAllDocuments"; 
    public const string UpdateDocument = "updateDocument"; 
    public const string DeleteDocument = "deleteDocument"; 
    public const string GetFolderWithDetails = "getFolderWithDetails"; 
    
    public const string BlazorLocalStorageFirebaseConfigName = "FireBaseConfig";
    
    // Document Root Path
    public const string VerifyTest = "verifyingtest"; 
    public const string FolderParentPath = "folderparentpath"; 
    public const string NoteParentPath = "noteparentpath"; 
    public const string ShortCutParentPath = "shortcutparentpath"; 
    public const string TagsParentPath = "tagsparentpath";  
    
    
    // Regex Pattern
    public const string FirebaseConfigPattern = @"(?<key>[\w]+)\s*:\s*""(?<value>[^""]+)""";
    
    
    // Text for Ui
    public const string FirebaseConfigText = "Firebase Config";
    public const string FirebaseConfigTestInProgress = "Testing in Progress...";
    public const string FirebaseConfigValidateAndTest = "Validate and Test";
    public const string FirebaseConfigInitiationSuccess = "Firebase initialized successfully.";
    public const string FirebaseConfigInitiationFailed = "Failed to initialize Firebase.";
    public const string FirebaseConfigTestAgain = "Test Again";
    public const string FirebaseConfigTestFailed = "Test Failed";
    
    // style for UI
    public const string DialogueUiStyle =
        """width:600px; padding:16px; background-color:#ffffffd6;border:4px solid #ffffffb3; border-radius:30px;box-shadow: inset  0 0 10px rgba(0, 0, 0, 0.1);""";
    public static readonly List<string> IconsList =
    [
        "fas fa-atom",
        "fas fa-folder-open",
        "fas fa-folder-plus",
        "fas fa-folder-minus",
    ]; 
}