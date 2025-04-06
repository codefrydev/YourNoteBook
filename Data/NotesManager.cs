using YourNoteBook.Models;
using YourNoteBook.Services;

namespace YourNoteBook.Data;

public class NotesManager(IFirebaseJsInteropService firebaseJsInteropService) :IManager<Note>
{   
    public Task<List<Note>> GetAllSync()
    {
        throw new NotImplementedException();
    }

    public Task<Note> GetByIdSync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<T> AddSync<T>(Note item)
    {
        throw new NotImplementedException();
    }

    public Task<T> UpdateSync<T>(Note item)
    {
        throw new NotImplementedException();
    }

    public Task<T> DeleteSync<T>(string id)
    {
        throw new NotImplementedException();
    }
}