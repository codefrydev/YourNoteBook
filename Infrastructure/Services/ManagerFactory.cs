using YourNoteBook.Core.Interfaces;
using YourNoteBook.Core.Entities;
using YourNoteBook.Core.Abstractions;
using YourNoteBook.Infrastructure.Services.Firebase;

namespace YourNoteBook.Infrastructure.Services;

/// <summary>
/// Factory implementation for creating manager instances
/// </summary>
public class ManagerFactory : IManagerFactory
{
    private readonly IFirebaseJsInteropService _firebaseService;

    public ManagerFactory(IFirebaseJsInteropService firebaseService)
    {
        _firebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
    }

    public IManager<Note> CreateNotesManager()
    {
        return new NotesManager(_firebaseService);
    }

    public IManager<Folder> CreateFoldersManager()
    {
        return new FoldersManager(_firebaseService);
    }

    public IManager<Shortcut> CreateShortcutsManager()
    {
        return new ShortcutsManager(_firebaseService);
    }

    public IManager<Tag> CreateTagsManager()
    {
        return new TagsManager(_firebaseService);
    }

    public IManager<Category> CreateCategoriesManager()
    {
        return new CategoriesManager(_firebaseService);
    }
}

/// <summary>
/// Manager for Notes
/// </summary>
public class NotesManager : BaseManager<Note>
{
    public NotesManager(IFirebaseJsInteropService firebaseService) 
        : base(firebaseService, "notes") { }
}

/// <summary>
/// Manager for Folders
/// </summary>
public class FoldersManager : BaseManager<Folder>
{
    public FoldersManager(IFirebaseJsInteropService firebaseService) 
        : base(firebaseService, "folders") { }
}

/// <summary>
/// Manager for Shortcuts
/// </summary>
public class ShortcutsManager : BaseManager<Shortcut>
{
    public ShortcutsManager(IFirebaseJsInteropService firebaseService) 
        : base(firebaseService, "shortcuts") { }
}

/// <summary>
/// Manager for Tags
/// </summary>
public class TagsManager : BaseManager<Tag>
{
    public TagsManager(IFirebaseJsInteropService firebaseService) 
        : base(firebaseService, "tags") { }
}

/// <summary>
/// Manager for Categories
/// </summary>
public class CategoriesManager : BaseManager<Category>
{
    public CategoriesManager(IFirebaseJsInteropService firebaseService) 
        : base(firebaseService, "categories") { }
}
