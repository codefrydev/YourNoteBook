using System.Text.Json;
using Microsoft.JSInterop;
using YourNoteBook.Models;
using YourNoteBook.Pages;
using YourNoteBook.Services;
using YourNoteBook.Utils;

namespace YourNoteBook.Data;

public class FolderManger(IFirebaseJsInteropService firebaseJsInteropService): IManager<FolderModel>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task<List<FolderModel>> GetAllSync()
    {
        var result = await firebaseJsInteropService.GetAllAsync<JsonElement>(Constant.FolderParentPath);
       
        return result.EnumerateArray()
            .Select(contactElement => JsonSerializer.Deserialize<FolderModel>(contactElement.GetRawText(), _jsonSerializerOptions))
            .OfType<FolderModel>().ToList();
    }
    
    public async Task<FolderModel> GetByIdSync(string id)
    {
        var result = await firebaseJsInteropService.GetAsync<JsonElement>(Constant.FolderParentPath, id);

        if (result.ValueKind != JsonValueKind.Object)
            return null!;

        return JsonSerializer.Deserialize<FolderModel>(result.GetRawText(), _jsonSerializerOptions)??null!;
    }


    public async  Task<T> AddSync<T>(FolderModel item)
    {
        item.Created = DateTime.Now; 
        var formData = Mapper.GetFolderModelLocalObject(item);
        return await firebaseJsInteropService.SaveAsync<T>(Constant.FolderParentPath, formData);
    }

    public async Task<T> UpdateSync<T>(FolderModel item)
    {
        item.UpdatedAt = DateTime.Now;
        var formData = Mapper.GetFolderModelLocalObject(item);Mapper.GetFolderModelLocalObject(item);
        return await firebaseJsInteropService.UpdateAsync<T>(Constant.FolderParentPath, item.Id, formData);
    }

    public async Task<T> DeleteSync<T>(string id)
    {
        return await firebaseJsInteropService.DeleteAsync<T>(Constant.FolderParentPath, id);
    }
}