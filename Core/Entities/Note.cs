using System.Text.Json;
using System.Text.Json.Serialization;
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
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Content/body of the note
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Tags associated with this note
    /// </summary>
    [JsonPropertyName("tags")]
    public List<Tag> Tags { get; set; } = new();
    
    /// <summary>
    /// Identifier of the folder containing this note
    /// </summary>
    [JsonPropertyName("folderId")]
    public string FolderId { get; set; } = string.Empty;
    
    /// <summary>
    /// When the note was created
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime Created { get; set; } = DateTime.Now;
    
    /// <summary>
    /// When the note was last modified
    /// </summary>
    [JsonPropertyName("modified")]
    public DateTime Modified { get; set; } = DateTime.Now;
}