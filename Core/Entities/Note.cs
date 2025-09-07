using System.Text.Json;
using YourNoteBook.Core.Abstractions;

namespace YourNoteBook.Core.Entities;

/// <summary>
/// Represents a note entity in the system
/// </summary>
public class Note : BaseEntity
{
    /// <summary>
    /// Unique identifier for the note
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Title of the note
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Content/body of the note
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Tags associated with this note
    /// </summary>
    public List<Tag> Tags { get; set; } = new();
    
    /// <summary>
    /// Identifier of the folder containing this note
    /// </summary>
    public string FolderId { get; set; } = string.Empty;
    
    /// <summary>
    /// When the note was created
    /// </summary>
    public DateTime Created { get; set; } = DateTime.Now;
    
    /// <summary>
    /// When the note was last modified
    /// </summary>
    public DateTime Modified { get; set; } = DateTime.Now;
}