namespace YourNoteBook.Models;
using System;
using System.Collections.Generic; 
public class Folder
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.Now;
    public List<Shortcut> Shortcuts { get; set; } = new List<Shortcut>();
    public List<Note> Notes { get; set; } = new List<Note>();
}

public class Note
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public List<Tag> Tags { get; set; } = new List<Tag>();  
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Modified { get; set; } = DateTime.Now;
    public string FolderId { get; set; } = "";
}

public class Tag
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
}

public class Shortcut
{
    public string Id { get; set; } = "";
    public string Action { get; set; } = "";
    public string Keys { get; set; } = "";
    public string Description { get; set; } = "";
    public Category Category { get; set; } = new Category();
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Modified { get; set; } = DateTime.Now;
    public string FolderId { get; set; } = "";
}

public class Category
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
}
public class FirebaseFolderResponse
{
    public bool Success { get; set; }
    public string Id { get; set; } = "";
    public string Error { get; set; } = "";
}