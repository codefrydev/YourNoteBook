using YourNoteBook.Models;
using YourNoteBook.Services;

namespace YourNoteBook.Data;

public class ShortcutManager(IFirebaseJsInteropService firebaseJsInteropService):IManager<Shortcut>
{
    public Task<List<Shortcut>> GetAllSync()
    {
        throw new NotImplementedException();
    }

    public Task<Shortcut> GetByIdSync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<T> AddSync<T>(Shortcut item)
    {
        throw new NotImplementedException();
    }

    public Task<T> UpdateSync<T>(Shortcut item)
    {
        throw new NotImplementedException();
    }

    public Task<T> DeleteSync<T>(string id)
    {
        throw new NotImplementedException();
    }
}