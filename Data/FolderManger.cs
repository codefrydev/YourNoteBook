using YourNoteBook.Models;

namespace YourNoteBook.Data;

public class FolderManger: IManager<Folder>
{
    public Task<List<Folder>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<Folder> GetById(string id)
    {
        throw new NotImplementedException();
    }

    public Task Add(Folder item)
    {
        throw new NotImplementedException();
    }

    public Task Update(Folder item)
    {
        throw new NotImplementedException();
    }

    public Task Delete(string id)
    {
        throw new NotImplementedException();
    }
}