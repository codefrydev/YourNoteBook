using YourNoteBook.Core.Entities;

namespace YourNoteBook.Core.Interfaces;

/// <summary>
/// Generic interface for managing data operations with Firebase
/// </summary>
/// <typeparam name="T">The entity type to manage</typeparam>
public interface IManager<T> where T : class
{
    /// <summary>
    /// Retrieves all entities from the data store
    /// </summary>
    /// <returns>A list of all entities</returns>
    Task<List<T>> GetAllSync();

    /// <summary>
    /// Retrieves an entity by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the entity</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<T?> GetByIdSync(string id);

    /// <summary>
    /// Adds a new entity to the data store
    /// </summary>
    /// <typeparam name="TU">The response type</typeparam>
    /// <param name="item">The entity to add</param>
    /// <returns>The response containing the result of the operation</returns>
    Task<TU> AddSync<TU>(T item) where TU : class;

    /// <summary>
    /// Updates an existing entity in the data store
    /// </summary>
    /// <typeparam name="TU">The response type</typeparam>
    /// <param name="item">The entity to update</param>
    /// <returns>The response containing the result of the operation</returns>
    Task<TU> UpdateSync<TU>(T item) where TU : class;

    /// <summary>
    /// Deletes an entity from the data store
    /// </summary>
    /// <typeparam name="TU">The response type</typeparam>
    /// <param name="id">The unique identifier of the entity to delete</param>
    /// <returns>The response containing the result of the operation</returns>
    Task<TU> DeleteSync<TU>(string id) where TU : class;
}