using YourNoteBook.Models;

namespace YourNoteBook.Data;

public interface IManager<T>
{
    Task<List<T>> GetAll();
    Task<T> GetById(string id);
    Task Add(T item);
    Task Update(T item);
    Task Delete(string id); 
}