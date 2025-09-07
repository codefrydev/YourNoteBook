using YourNoteBook.Core.Entities;
using YourNoteBook.Shared.Models.SEO;
using YourNoteBook.Shared.Components.Base;

namespace YourNoteBook.Shared.Services.SEO;

public interface IDynamicSeoService
{
    Task<SeoMetadata> GenerateNoteSeoAsync(Note note, Folder? folder = null);
    Task<SeoMetadata> GenerateFolderSeoAsync(Folder folder, List<Note> notes);
    Task<ArticleJsonLd> GenerateNoteJsonLdAsync(Note note, Folder? folder = null);
    Task<WebPageJsonLd> GenerateFolderJsonLdAsync(Folder folder, List<Note> notes);
    Task<BreadcrumbListJsonLd> GenerateBreadcrumbAsync(string currentPage, string? folderName = null, string? noteTitle = null);
}

public class DynamicSeoService : IDynamicSeoService
{
    public async Task<SeoMetadata> GenerateNoteSeoAsync(Note note, Folder? folder = null)
    {
        var folderName = folder?.Name ?? "Uncategorized";
        var title = $"{note.Title} - {folderName} | YourNoteBook";
        
        // Generate description from note content (first 160 characters)
        var description = note.Content?.Length > 160 
            ? note.Content.Substring(0, 157) + "..." 
            : note.Content ?? "A note in YourNoteBook";
            
        var tagNames = note.Tags?.Select(t => t.Name).ToList() ?? new List<string>();
        var keywords = GenerateKeywordsFromContent(note.Content, tagNames, folderName);
        
        return new SeoMetadata
        {
            Title = title,
            Description = description,
            Keywords = keywords,
            ImageUrl = "https://codefrydev.in/YourNoteBook/icon-192.png",
            Url = $"https://codefrydev.in/YourNoteBook/{note.Id}"
        };
    }

    public async Task<SeoMetadata> GenerateFolderSeoAsync(Folder folder, List<Note> notes)
    {
        var noteCount = notes.Count;
        var title = $"{folder.Name} - {noteCount} Notes | YourNoteBook";
        
        var description = !string.IsNullOrEmpty(folder.Description) 
            ? folder.Description
            : $"Browse {noteCount} notes in the {folder.Name} folder. Organize your thoughts and ideas with YourNoteBook.";
            
        var keywords = GenerateKeywordsFromContent(folder.Description, new List<string>(), folder.Name);
        
        return new SeoMetadata
        {
            Title = title,
            Description = description,
            Keywords = keywords,
            ImageUrl = "https://codefrydev.in/YourNoteBook/icon-192.png",
            Url = $"https://codefrydev.in/YourNoteBook/folder/{folder.Id}"
        };
    }

    public async Task<ArticleJsonLd> GenerateNoteJsonLdAsync(Note note, Folder? folder = null)
    {
        var folderName = folder?.Name ?? "Uncategorized";
        
        return new ArticleJsonLd
        {
            Headline = note.Title,
            Description = note.Content?.Length > 160 
                ? note.Content.Substring(0, 157) + "..." 
                : note.Content ?? "A note in YourNoteBook",
            Url = $"https://codefrydev.in/YourNoteBook/note/{note.Id}",
            DatePublished = note.Created.ToString("yyyy-MM-dd"),
            DateModified = note.Modified.ToString("yyyy-MM-dd"),
            Keywords = note.Tags?.Select(t => t.Name).ToList() ?? new List<string>(),
            ArticleBody = note.Content ?? ""
        };
    }

    public async Task<WebPageJsonLd> GenerateFolderJsonLdAsync(Folder folder, List<Note> notes)
    {
        var noteCount = notes.Count;
        var title = $"{folder.Name} - {noteCount} Notes | YourNoteBook";
        
        return new WebPageJsonLd
        {
            Name = title,
            Description = !string.IsNullOrEmpty(folder.Description) 
                ? folder.Description
                : $"Browse {noteCount} notes in the {folder.Name} folder.",
            Url = $"https://codefrydev.in/YourNoteBook/folder/{folder.Id}",
            DatePublished = folder.Created.ToString("yyyy-MM-dd"),
            DateModified = folder.UpdatedAt.ToString("yyyy-MM-dd")
        };
    }

    public async Task<BreadcrumbListJsonLd> GenerateBreadcrumbAsync(string currentPage, string? folderName = null, string? noteTitle = null)
    {
        var breadcrumbs = new List<BreadcrumbItemJsonLd>
        {
            new() { Position = 1, Name = "Home", Item = "https://codefrydev.in/YourNoteBook" }
        };

        var position = 2;

        if (!string.IsNullOrEmpty(folderName))
        {
            breadcrumbs.Add(new BreadcrumbItemJsonLd
            {
                Position = position++,
                Name = folderName,
                Item = $"https://codefrydev.in/YourNoteBook/folder/{folderName.ToLower().Replace(" ", "-")}"
            });
        }

        if (!string.IsNullOrEmpty(noteTitle))
        {
            breadcrumbs.Add(new BreadcrumbItemJsonLd
            {
                Position = position++,
                Name = noteTitle,
                Item = $"https://codefrydev.in/YourNoteBook/note/{noteTitle.ToLower().Replace(" ", "-")}"
            });
        }

        breadcrumbs.Add(new BreadcrumbItemJsonLd
        {
            Position = position,
            Name = currentPage,
            Item = $"https://codefrydev.in/YourNoteBook/{currentPage.ToLower()}"
        });

        return new BreadcrumbListJsonLd
        {
            ItemListElement = breadcrumbs
        };
    }

    private string GenerateKeywordsFromContent(string? content, List<string>? tags, string folderName)
    {
        var keywords = new List<string>
        {
            "YourNoteBook",
            "notes",
            "productivity",
            "organization"
        };

        if (!string.IsNullOrEmpty(folderName))
        {
            keywords.Add(folderName.ToLower());
        }

        if (tags != null && tags.Any())
        {
            keywords.AddRange(tags.Select(t => t.ToLower()));
        }

        if (!string.IsNullOrEmpty(content))
        {
            // Extract potential keywords from content (simple approach)
            var words = content.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 3)
                .GroupBy(w => w.ToLower())
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key);
                
            keywords.AddRange(words);
        }

        return string.Join(", ", keywords.Distinct().Take(10));
    }
}
