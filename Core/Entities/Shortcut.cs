using YourNoteBook.Core.Abstractions;

namespace YourNoteBook.Core.Entities;

/// <summary>
/// Represents a keyboard shortcut entity in the system
/// </summary>
public class Shortcut : BaseEntity
{
    /// <summary>
    /// Unique identifier for the shortcut
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Action performed by the shortcut
    /// </summary>
    public string Action { get; set; } = string.Empty;
    
    /// <summary>
    /// Keyboard keys for the shortcut
    /// </summary>
    public string Keys { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of what the shortcut does
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Category this shortcut belongs to
    /// </summary>
    public Category? Category { get; set; }
    
    /// <summary>
    /// Identifier of the folder containing this shortcut
    /// </summary>
    public string FolderId { get; set; } = string.Empty;
    
    /// <summary>
    /// When the shortcut was created
    /// </summary>
    public DateTime Created { get; set; } = DateTime.Now;
    
    /// <summary>
    /// When the shortcut was last modified
    /// </summary>
    public DateTime Modified { get; set; } = DateTime.Now;
}