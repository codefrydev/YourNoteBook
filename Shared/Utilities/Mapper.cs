using System.Text.Json;
using YourNoteBook.Core.Entities;

namespace YourNoteBook.Shared.Utilities;

public static class Mapper
{  
    public static object GetFolderLocalObject(Folder item)
    {
        return new 
        {
            id = item.Id,  
            createdAt = item.CreatedAt,
            modifiedAt = item.ModifiedAt,
            name = item.Name,
            icon = item.Icon,
            iconColor = item.IconColor,
            backgroundColor = item.BackgroundColor,
            isPinned = item.IsPinned,
            description = item.Description,
        };
    }
    public static object GetNoteLocalObject(Note item)
    {
        return new 
        {
            id = item.Id, 
            content = item.Content,
            createdAt = item.CreatedAt,
            modifiedAt = item.ModifiedAt,
            folderId = item.FolderId,
            title = item.Title,
            tags = item.Tags,
            icon = item.Icon,
            iconColor = item.IconColor,
            backgroundColor = item.BackgroundColor,
            isPinned = item.IsPinned,
        };
    }
    public static object GetShortCutLocalObject(Shortcut item)
    {
        return new 
        {
            id = item.Id,  
            createdAt = item.CreatedAt,
            modifiedAt = item.ModifiedAt,
            folderId = item.FolderId,
            category = item.Category,
            action = item.Action,
            keys = item.Keys,
            description = item.Description,
            icon = item.Icon,
            iconColor = item.IconColor,
            backgroundColor = item.BackgroundColor,
            isPinned = item.IsPinned,
        };
    }
} 