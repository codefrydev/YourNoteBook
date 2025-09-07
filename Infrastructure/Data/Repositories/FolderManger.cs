using System.Text.Json;
using Microsoft.JSInterop;
using YourNoteBook.Core.Entities;
using YourNoteBook.Core.Interfaces;
using YourNoteBook.Infrastructure.Services.Firebase;
using YourNoteBook.Shared.Utilities;

namespace YourNoteBook.Infrastructure.Data.Repositories;

public class FolderManger(IFirebaseJsInteropService firebaseJsInteropService): IManager<Folder>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task<List<Folder>> GetAllSync()
    {
        var result = await firebaseJsInteropService.GetAllAsync<JsonElement>(Constant.FolderParentPath);
       
        return result.EnumerateArray()
            .Select(contactElement => JsonSerializer.Deserialize<Folder>(contactElement.GetRawText(), _jsonSerializerOptions))
            .OfType<Folder>().ToList();
    }
    
    public async Task<Folder> GetByIdSync(string id)
    {
        var result = await firebaseJsInteropService.GetAsync<JsonElement>(Constant.FolderParentPath, id);

        if (result.ValueKind != JsonValueKind.Object)
            return null!;

        return JsonSerializer.Deserialize<Folder>(result.GetRawText(), _jsonSerializerOptions) ?? null!;
    }


    async Task<TU> IManager<Folder>.AddSync<TU>(Folder item)
    {
        item.CreatedAt = DateTime.UtcNow; 
        var formData = Mapper.GetFolderLocalObject(item);
        return await firebaseJsInteropService.SaveAsync<TU>(Constant.FolderParentPath, formData);
    }

    async Task<TU> IManager<Folder>.UpdateSync<TU>(Folder item)
    {
        item.ModifiedAt = DateTime.UtcNow;
        var formData = Mapper.GetFolderLocalObject(item);
        return await firebaseJsInteropService.UpdateAsync<TU>(Constant.FolderParentPath, item.Id, formData);
    }

    async Task<TU> IManager<Folder>.DeleteSync<TU>(string id)
    {
        return await firebaseJsInteropService.DeleteAsync<TU>(Constant.FolderParentPath, id);
    }
}