using System.Text.Json;
using Microsoft.JSInterop;
using YourNoteBook.Models;
using YourNoteBook.Pages;
using YourNoteBook.Services;
using YourNoteBook.Utils;

namespace YourNoteBook.Data;

public class FolderManger(IFirebaseJsInteropService firebaseJsInteropService,IJSRuntime jsRuntime): IManager<FolderModel>
{
    public async Task<List<FolderModel>> GetAllSync()
    {
        var result = await firebaseJsInteropService.GetAllAsync<JsonElement>(Constant.FolderParentPath); 
        var list = new List<FolderModel>();  
        foreach (var contactElement in result.EnumerateArray())
        {
            var folder = new FolderModel()
            {
                Id = contactElement.GetProperty("id").GetString()!,
                Name = contactElement.GetProperty("name").GetString()!
            };
            list.Add(folder);
        }

        return list;
    }
    

    public Task<FolderModel> GetByIdSync(string id)
    {
        throw new NotImplementedException();
    }

    public async  Task<T> AddSync<T>(FolderModel item)
    {
        var formData = new
        {
            id = item.Id,
            name = item.Name, 
            createdDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
            updatedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
        };
        return await firebaseJsInteropService.SaveAsync<T>(Constant.FolderParentPath, formData);
    }

    public Task<T> UpdateSync<T>(FolderModel item)
    {
        throw new NotImplementedException();
    }

    public Task<T> DeleteSync<T>(string id)
    {
        throw new NotImplementedException();
    }
}