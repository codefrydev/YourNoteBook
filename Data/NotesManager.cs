using YourNoteBook.Models;

namespace YourNoteBook.Data;

public class NotesManager :IManager<Note>
{   
    public Task<List<Note>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<Note> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public Task Add(Note item)
    {
        throw new NotImplementedException();
    }

    public Task Update(Note item)
    {
        throw new NotImplementedException();
    }

    public Task Delete(string id)
    {
        throw new NotImplementedException();
    }
}