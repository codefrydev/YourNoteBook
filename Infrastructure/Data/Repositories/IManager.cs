using YourNoteBook.Core.Entities;

namespace YourNoteBook.Infrastructure.Data.Repositories;

public interface IManager<T>
{
    Task<List<T>> GetAllSync();
    Task<T> GetByIdSync(string id);
    Task<TU> AddSync<TU>(T item);
    Task<TU> UpdateSync<TU>(T item);
    Task<TU> DeleteSync<TU>(string id); 
}