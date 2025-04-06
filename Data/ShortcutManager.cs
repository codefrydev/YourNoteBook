using System.Text.Json;
using YourNoteBook.Models;
using YourNoteBook.Services;
using YourNoteBook.Utils;

namespace YourNoteBook.Data;

public class ShortcutManager(IFirebaseJsInteropService firebaseJsInteropService):IManager<Shortcut>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task<List<Shortcut>> GetAllSync()
    {
        var result = await firebaseJsInteropService.GetAllAsync<JsonElement>(Constant.ShortCutParentPath);   
        return result.EnumerateArray()
            .Select(contactElement => JsonSerializer.Deserialize<Shortcut>(contactElement.GetRawText(), _jsonSerializerOptions))
            .OfType<Shortcut>().ToList();
    }
    
    public async Task<Shortcut> GetByIdSync(string id)
    {
        var result = await firebaseJsInteropService.GetAsync<JsonElement>(Constant.ShortCutParentPath, id);

        if (result.ValueKind != JsonValueKind.Object)
            return null!;

        return JsonSerializer.Deserialize<Shortcut>(result.GetRawText(), _jsonSerializerOptions)??null!;
    }


    public async  Task<T> AddSync<T>(Shortcut item)
    {
        var formData = Mapper.GetShortCutLocalObject(item);
        return await firebaseJsInteropService.SaveAsync<T>(Constant.ShortCutParentPath, formData);
    }

    public async Task<T> UpdateSync<T>(Shortcut item)
    {
        var formData = Mapper.GetShortCutLocalObject(item);
        return await firebaseJsInteropService.UpdateAsync<T>(Constant.ShortCutParentPath, item.Id, formData);
    }

    public async Task<T> DeleteSync<T>(string id)
    {
        return await firebaseJsInteropService.DeleteAsync<T>(Constant.ShortCutParentPath, id);
    }
}