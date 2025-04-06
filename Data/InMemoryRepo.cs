using YourNoteBook.Models;

namespace YourNoteBook.Data;

public class InMemoryRepo
{
    // Event to notify when data changes
    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
    public List<Note> Notes { get; set; } = [];
    public List<FolderModel> Folders { get; set; } = [];
    public List<Shortcut> Shortcuts { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
    public List<Category> Categories { get; set; } = [];

    public Note? GetNoteById(string id) => Notes.Single(x => x.Id == id);

    public FolderModel? GetFolderById(string id) => Folders.Single(x => x.Id == id);
    public Shortcut? GetShortcutById(string id) => Shortcuts.Single(x => x.Id == id);
    public Tag? GetTagById(string id) => Tags.Single(x => x.Id == id);
    public Category? GetCategoryById(string id) => Categories.Single(x => x.Id == id);

    public List<Note> GetNotesByFolderId(string folderId) => Notes.Where(x => x.FolderId == folderId).ToList();

    public List<Shortcut> GetShortcutsByFolderId(string folderId) =>
        Shortcuts.Where(x => x.FolderId == folderId).ToList();

    public List<Note> GetNotesByTagId(string tagId) => Notes.Where(x => x.Tags.Any(t => t.Id == tagId)).ToList();

    public void AddItem<T>(T item) where T : class
    {
        switch (item)
        {
            case Note note:
                Notes.Add(note);
                break;
            case FolderModel folder:
                Folders.Add(folder);
                break;
            case Shortcut shortcut:
                Shortcuts.Add(shortcut);
                break;
            case Tag tag:
                Tags.Add(tag);
                break;
            case Category category:
                Categories.Add(category);
                break;
        }

        NotifyStateChanged();
    }

    public void DeleteNotesByNotesId(string id)
    {
        var note = GetNoteById(id);
        if (note == null) return;
        Notes.Remove(note);
        NotifyStateChanged();
    }

    public void DeleteShortcutsByShortcutId(string shortcutId)
    {
        var shortcut = GetShortcutById(shortcutId);
        if (shortcut == null) return;
        Shortcuts.Remove(shortcut);
        NotifyStateChanged();
    }

    public void DeleteFoldersByFolderId(string id)
    {
        var folder = GetFolderById(id);
        foreach (var noteId in GetNotesByFolderId(id))
        {
            DeleteNotesByNotesId(noteId.Id);
        }

        foreach (var shortcutId in GetShortcutsByFolderId(id))
        {
            DeleteShortcutsByShortcutId(shortcutId.Id);
        }

        if (folder == null) return;
        Folders.Remove(folder);
        NotifyStateChanged();
    }
}