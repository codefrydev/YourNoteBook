using System.Text.Json;
using YourNoteBook.Models;

namespace YourNoteBook.Utils;

public static class Mapper
{  
    public static object GetFolderModelLocalObject(FolderModel item)
    {
        return new 
        {
            id = item.Id,  
            created =  item.Created,
            updatedAt =   item.UpdatedAt,
            name =  item.Name, 
        };
    }
    public static object GetNoteLocalObject(Note item)
    {
        return new 
        {
            id = item.Id, 
            content =  item.Content,
            created =  item.Created,
            modified =   item.Modified,
            folderId =  item.FolderId,
            title =   item.Title,
            tags =  item.Tags,
        };
    }
    public static object GetShortCutLocalObject(Shortcut item)
    {
        return new 
        {
            id = item.Id,  
            created =  item.Created,
            modified =   item.Modified,
            folderId =  item.FolderId,
            category = item.Category,
            action =   item.Action,
            keys =   item.Keys,
            description =   item.Description,
        };
    }
} 