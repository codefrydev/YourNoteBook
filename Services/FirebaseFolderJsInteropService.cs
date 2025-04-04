namespace YourNoteBook.Services;
 
using System.Threading.Tasks;
using Microsoft.JSInterop;

public class FirebaseFolderJsInteropService
{
    private readonly IJSRuntime _jsRuntime;

    public FirebaseFolderJsInteropService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    // Initialize Firebase with a configuration object.
    public async Task<bool> InitializeFirebaseAsync(object config)
    {
        return await _jsRuntime.InvokeAsync<bool>("initializeFirebase", config);
    }

    // Create a new Folder.
    public async Task<dynamic> SaveFolderAsync(object folder)
    {
        return await _jsRuntime.InvokeAsync<dynamic>("saveFolder", folder);
    }

    // Retrieve a Folder by its ID.
    public async Task<dynamic> GetFolderAsync(string folderId)
    {
        return await _jsRuntime.InvokeAsync<dynamic>("getFolder", folderId);
    }

    // Update an existing Folder.
    public async Task<dynamic> UpdateFolderAsync(string folderId, object folder)
    {
        return await _jsRuntime.InvokeAsync<dynamic>("updateFolder", folderId, folder);
    }

    // Delete a Folder.
    public async Task<dynamic> DeleteFolderAsync(string folderId)
    {
        return await _jsRuntime.InvokeAsync<dynamic>("deleteFolder", folderId);
    }
}
