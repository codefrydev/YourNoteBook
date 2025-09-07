using YourNoteBook.Core.Entities;
using YourNoteBook.Core.Interfaces;
using YourNoteBook.Infrastructure.Services.Firebase;

namespace YourNoteBook.Core.Abstractions;

/// <summary>
/// Base implementation for data managers with common functionality
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
public abstract class BaseManager<T> : IManager<T> where T : class
{
    protected readonly IFirebaseJsInteropService FirebaseService;
    protected readonly string CollectionPath;

    protected BaseManager(IFirebaseJsInteropService firebaseService, string collectionPath)
    {
        FirebaseService = firebaseService ?? throw new ArgumentNullException(nameof(firebaseService));
        CollectionPath = collectionPath ?? throw new ArgumentNullException(nameof(collectionPath));
    }

    /// <summary>
    /// Retrieves all entities from the collection
    /// </summary>
    public virtual async Task<List<T>> GetAllSync()
    {
        try
        {
            Console.WriteLine($"BaseManager.GetAllSync: Retrieving {typeof(T).Name}s from collection '{CollectionPath}'");
            var result = await FirebaseService.GetAllAsync<List<T>>(CollectionPath);
            Console.WriteLine($"BaseManager.GetAllSync: Retrieved {result?.Count ?? 0} {typeof(T).Name}s from '{CollectionPath}'");
            
            if (result != null && result.Count > 0)
            {
                Console.WriteLine($"BaseManager.GetAllSync: First item type: {result[0]?.GetType()}");
                Console.WriteLine($"BaseManager.GetAllSync: First item: {result[0]}");
            }
            
            return result ?? new List<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BaseManager.GetAllSync: Error retrieving {typeof(T).Name}s from '{CollectionPath}': {ex.Message}");
            Console.WriteLine($"BaseManager.GetAllSync: Stack trace: {ex.StackTrace}");
            // Log the exception (you might want to inject ILogger here)
            throw new DataAccessException($"Failed to retrieve all {typeof(T).Name}s", ex);
        }
    }

    /// <summary>
    /// Retrieves an entity by its ID
    /// </summary>
    public virtual async Task<T?> GetByIdSync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        try
        {
            return await FirebaseService.GetAsync<T>(CollectionPath, id);
        }
        catch (Exception ex)
        {
            throw new DataAccessException($"Failed to retrieve {typeof(T).Name} with ID: {id}", ex);
        }
    }

    /// <summary>
    /// Adds a new entity to the collection
    /// </summary>
    public virtual async Task<TU> AddSync<TU>(T item) where TU : class
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        try
        {
            return await FirebaseService.SaveAsync<TU>(CollectionPath, item);
        }
        catch (Exception ex)
        {
            throw new DataAccessException($"Failed to add {typeof(T).Name}", ex);
        }
    }

    /// <summary>
    /// Updates an existing entity
    /// </summary>
    public virtual async Task<TU> UpdateSync<TU>(T item) where TU : class
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        try
        {
            // Extract ID from the item (assuming it has an Id property)
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty?.GetValue(item) is not string id || string.IsNullOrWhiteSpace(id))
                throw new InvalidOperationException($"Item must have a valid Id property");

            return await FirebaseService.UpdateAsync<TU>(CollectionPath, id, item);
        }
        catch (Exception ex)
        {
            throw new DataAccessException($"Failed to update {typeof(T).Name}", ex);
        }
    }

    /// <summary>
    /// Deletes an entity by its ID
    /// </summary>
    public virtual async Task<TU> DeleteSync<TU>(string id) where TU : class
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        try
        {
            return await FirebaseService.DeleteAsync<TU>(CollectionPath, id);
        }
        catch (Exception ex)
        {
            throw new DataAccessException($"Failed to delete {typeof(T).Name} with ID: {id}", ex);
        }
    }
}

/// <summary>
/// Custom exception for data access operations
/// </summary>
public class DataAccessException : Exception
{
    public DataAccessException(string message) : base(message) { }
    public DataAccessException(string message, Exception innerException) : base(message, innerException) { }
}
