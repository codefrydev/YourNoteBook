using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace YourNoteBook.Shared.Services.SEO;

public interface ISeoMetadataService
{
    Task SetPageMetadataAsync(string title, string description, string keywords = "", string imageUrl = "", string url = "");
    Task SetJsonLdAsync(object jsonLdData);
    Task SetOpenGraphAsync(string title, string description, string imageUrl = "", string url = "");
    Task SetTwitterCardAsync(string title, string description, string imageUrl = "", string url = "");
}

public class SeoMetadataService : ISeoMetadataService
{
    private readonly IJSRuntime _jsRuntime;

    public SeoMetadataService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetPageMetadataAsync(string title, string description, string keywords = "", string imageUrl = "", string url = "")
    {
        await _jsRuntime.InvokeVoidAsync("setPageMetadata", new
        {
            title = title,
            description = description,
            keywords = keywords,
            imageUrl = imageUrl,
            url = url
        });
    }

    public async Task SetJsonLdAsync(object jsonLdData)
    {
        var jsonString = JsonSerializer.Serialize(jsonLdData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await _jsRuntime.InvokeVoidAsync("setJsonLd", jsonString);
    }

    public async Task SetOpenGraphAsync(string title, string description, string imageUrl = "", string url = "")
    {
        await _jsRuntime.InvokeVoidAsync("setOpenGraph", new
        {
            title = title,
            description = description,
            imageUrl = imageUrl,
            url = url
        });
    }

    public async Task SetTwitterCardAsync(string title, string description, string imageUrl = "", string url = "")
    {
        await _jsRuntime.InvokeVoidAsync("setTwitterCard", new
        {
            title = title,
            description = description,
            imageUrl = imageUrl,
            url = url
        });
    }
}
