namespace YourNoteBook.Shared.Models.SEO;

public class WebSiteJsonLd
{
    public string Context { get; set; } = "https://schema.org";
    public string Type { get; set; } = "WebSite";
    public string Name { get; set; } = "YourNoteBook";
    public string Description { get; set; } = "A modern note-taking application for organizing your thoughts and ideas";
    public string Url { get; set; } = "https://yournotebook.com";
    public string PotentialAction { get; set; } = "SearchAction";
    public string Target { get; set; } = "https://yournotebook.com/search?q={search_term_string}";
    public string QueryInput { get; set; } = "required name=search_term_string";
}

public class WebPageJsonLd
{
    public string Context { get; set; } = "https://schema.org";
    public string Type { get; set; } = "WebPage";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Url { get; set; } = "";
    public string DatePublished { get; set; } = "";
    public string DateModified { get; set; } = "";
    public PersonJsonLd Author { get; set; } = new();
    public OrganizationJsonLd Publisher { get; set; } = new();
}

public class ArticleJsonLd
{
    public string Context { get; set; } = "https://schema.org";
    public string Type { get; set; } = "Article";
    public string Headline { get; set; } = "";
    public string Description { get; set; } = "";
    public string Url { get; set; } = "";
    public string DatePublished { get; set; } = "";
    public string DateModified { get; set; } = "";
    public PersonJsonLd Author { get; set; } = new();
    public OrganizationJsonLd Publisher { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public string ArticleBody { get; set; } = "";
}

public class PersonJsonLd
{
    public string Type { get; set; } = "Person";
    public string Name { get; set; } = "YourNoteBook User";
    public string Url { get; set; } = "https://yournotebook.com";
}

public class OrganizationJsonLd
{
    public string Type { get; set; } = "Organization";
    public string Name { get; set; } = "YourNoteBook";
    public string Url { get; set; } = "https://yournotebook.com";
    public string Logo { get; set; } = "https://yournotebook.com/icon-192.png";
}

public class BreadcrumbListJsonLd
{
    public string Context { get; set; } = "https://schema.org";
    public string Type { get; set; } = "BreadcrumbList";
    public List<BreadcrumbItemJsonLd> ItemListElement { get; set; } = new();
}

public class BreadcrumbItemJsonLd
{
    public string Type { get; set; } = "ListItem";
    public int Position { get; set; }
    public string Name { get; set; } = "";
    public string Item { get; set; } = "";
}

public class SoftwareApplicationJsonLd
{
    public string Context { get; set; } = "https://schema.org";
    public string Type { get; set; } = "SoftwareApplication";
    public string Name { get; set; } = "YourNoteBook";
    public string Description { get; set; } = "A modern note-taking application for organizing your thoughts and ideas";
    public string Url { get; set; } = "https://yournotebook.com";
    public string ApplicationCategory { get; set; } = "ProductivityApplication";
    public string OperatingSystem { get; set; } = "Web Browser";
    public List<string> Offers { get; set; } = new() { "Free" };
    public OrganizationJsonLd Publisher { get; set; } = new();
}
