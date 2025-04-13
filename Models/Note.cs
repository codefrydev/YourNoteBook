using System.Text.Json;
using YourNoteBook.Utils;

namespace YourNoteBook.Models;
using System;
using System.Collections.Generic;

public class Note : BaseModel
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public List<Tag> Tags { get; set; } = [];  
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Modified { get; set; } = DateTime.Now;
    public string FolderId { get; set; } = ""; 
}