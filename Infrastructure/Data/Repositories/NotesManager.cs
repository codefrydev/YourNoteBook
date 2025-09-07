using System.Text.Json;
using YourNoteBook.Core.Entities;
using YourNoteBook.Infrastructure.Services.Firebase;
using YourNoteBook.Shared.Utilities;

namespace YourNoteBook.Infrastructure.Data.Repositories;

public class NotesManager(IFirebaseJsInteropService firebaseJsInteropService) :IManager<Note>
{   
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task<List<Note>> GetAllSync()
    {
        var result = await firebaseJsInteropService.GetAllAsync<JsonElement>(Constant.NoteParentPath);   
        return result.EnumerateArray()
            .Select(contactElement => JsonSerializer.Deserialize<Note>(contactElement.GetRawText(), _jsonSerializerOptions))
            .OfType<Note>().ToList();
    }
    
    public async Task<Note> GetByIdSync(string id)
    {
        var result = await firebaseJsInteropService.GetAsync<JsonElement>(Constant.NoteParentPath, id);

        if (result.ValueKind != JsonValueKind.Object)
            return null!;

        return JsonSerializer.Deserialize<Note>(result.GetRawText(), _jsonSerializerOptions)??null!;
    }


    public async  Task<T> AddSync<T>(Note item)
    {
        var formData = Mapper.GetNoteLocalObject(item);
        return await firebaseJsInteropService.SaveAsync<T>(Constant.NoteParentPath, formData);
    }

    public async Task<T> UpdateSync<T>(Note item)
    {
        var formData = Mapper.GetNoteLocalObject(item);
        return await firebaseJsInteropService.UpdateAsync<T>(Constant.NoteParentPath, item.Id, formData);
    }

    public async Task<T> DeleteSync<T>(string id)
    {
        return await firebaseJsInteropService.DeleteAsync<T>(Constant.NoteParentPath, id);
    }
}