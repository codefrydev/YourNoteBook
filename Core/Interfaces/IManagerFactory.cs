using YourNoteBook.Core.Entities;

namespace YourNoteBook.Core.Interfaces;

/// <summary>
/// Factory interface for creating manager instances
/// </summary>
public interface IManagerFactory
{
    IManager<Note> CreateNotesManager();
    IManager<Folder> CreateFoldersManager();
    IManager<Shortcut> CreateShortcutsManager();
    IManager<Tag> CreateTagsManager();
    IManager<Category> CreateCategoriesManager();
}
