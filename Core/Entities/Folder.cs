using YourNoteBook.Core.Abstractions;
using System.Text.Json.Serialization;

namespace YourNoteBook.Core.Entities;

/// <summary>
/// Represents a folder entity in the system
/// </summary>
public class Folder : BaseEntity
{
    /// <summary>
    /// Unique identifier for the folder
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the folder
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the folder
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Color of the folder (maps to 'color' from Firebase)
    /// </summary>
    [JsonPropertyName("color")]
    public string Color { get; set; } = "#3B82F6";
    
    /// <summary>
    /// When the folder was created (maps to 'created' from Firebase)
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime Created { get; set; } = DateTime.Now;
    
    /// <summary>
    /// When the folder was last updated (maps to 'updatedAt' from Firebase)
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}