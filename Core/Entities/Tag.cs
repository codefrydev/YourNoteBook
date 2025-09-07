using YourNoteBook.Core.Abstractions;

namespace YourNoteBook.Core.Entities;

/// <summary>
/// Represents a tag entity in the system
/// </summary>
public class Tag : BaseEntity
{
    /// <summary>
    /// Unique identifier for the tag
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Name of the tag
    /// </summary>
    public string Name { get; set; } = string.Empty;
}