using System.Text.Json.Serialization;
using YourNoteBook.Core.Abstractions;

namespace YourNoteBook.Core.Entities;

/// <summary>
/// Represents a category entity in the system
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Unique identifier for the category
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the category
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}