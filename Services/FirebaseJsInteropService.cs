using YourNoteBook.Utils;
using Microsoft.JSInterop;

namespace YourNoteBook.Services;

public interface IFirebaseJsInteropService
{
    Task<bool> InitializeFirebaseAsync(object config);
    Task<T> SaveAsync<T>(string parentPath, object folder);
    Task<T> GetAsync<T>(string parentPath,object itemIdToBeRetrieved);
    Task<T> UpdateAsync<T>(string parentpath, string folderId, object folder);
    Task<T> DeleteAsync<T>(string parentPath,object itemIdToBeDeleted);
}

public class FirebaseJsInteropService(IJSRuntime jsRuntime) : IFirebaseJsInteropService
{
    // Initialize Firebase with a configuration object.
    public async Task<bool> InitializeFirebaseAsync(object config)
    {
        return await jsRuntime.InvokeAsync<bool>(Constant.InitializeFirebase, config);
    }

    // Create a new Folder.
    public async Task<T> SaveAsync<T>(string parentPath,object itemToBeSaved)
    {
        return await jsRuntime.InvokeAsync<T>(Constant.SaveDocument, [parentPath,itemToBeSaved]);
    }

    // Retrieve a Folder by its ID.
    public async Task<T> GetAsync<T>(string parentPath,object itemIdToBeRetrieved)
    {
        return await jsRuntime.InvokeAsync<T>(Constant.GetDocument, [parentPath,itemIdToBeRetrieved]);
    }

    // Update an existing Folder.
    public async Task<T> UpdateAsync<T>(string parentpath, string folderId, object folder)
    {
        return await jsRuntime.InvokeAsync<T>(Constant.UpdateDocument, [parentpath,folderId, folder]);
    }

    // Delete a Folder.
    public async Task<T> DeleteAsync<T>(string parentPath,object itemIdToBeDeleted)
    {
        return await jsRuntime.InvokeAsync<T>(Constant.DeleteDocument, [parentPath,itemIdToBeDeleted]);
    }
}