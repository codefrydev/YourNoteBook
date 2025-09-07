using System.Text.Json.Serialization;

namespace YourNoteBook.Core.Abstractions;

/// <summary>
/// Base entity class providing common properties for all domain entities
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Icon identifier for the entity
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; } = string.Empty;
    
    /// <summary>
    /// Color of the icon
    /// </summary>
    [JsonPropertyName("iconColor")]
    public string? IconColor { get; set; } = string.Empty;
    
    /// <summary>
    /// Background color for the entity
    /// </summary>
    [JsonPropertyName("backgroundColor")]
    public string? BackgroundColor { get; set; } = string.Empty;
    
    /// <summary>
    /// Whether the entity is pinned/favorited
    /// </summary>
    [JsonPropertyName("isPinned")]
    public bool IsPinned { get; set; } = false;
    
    /// <summary>
    /// When the entity was created
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When the entity was last modified
    /// </summary>
    [JsonPropertyName("modifiedAt")]
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}