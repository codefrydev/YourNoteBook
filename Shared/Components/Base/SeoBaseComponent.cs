using Microsoft.AspNetCore.Components;
using YourNoteBook.Shared.Services.SEO;
using YourNoteBook.Shared.Models.SEO;

namespace YourNoteBook.Shared.Components.Base;

public abstract class SeoBaseComponent : ComponentBase
{
    [Inject] protected ISeoMetadataService SeoService { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetSeoMetadataAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected virtual async Task SetSeoMetadataAsync()
    {
        var metadata = GetSeoMetadata();
        if (metadata != null)
        {
            await SeoService.SetPageMetadataAsync(
                metadata.Title,
                metadata.Description,
                metadata.Keywords,
                metadata.ImageUrl,
                metadata.Url
            );

            await SeoService.SetOpenGraphAsync(
                metadata.Title,
                metadata.Description,
                metadata.ImageUrl,
                metadata.Url
            );

            await SeoService.SetTwitterCardAsync(
                metadata.Title,
                metadata.Description,
                metadata.ImageUrl,
                metadata.Url
            );

            var jsonLd = GetJsonLdData();
            if (jsonLd != null)
            {
                await SeoService.SetJsonLdAsync(jsonLd);
            }
        }
    }

    protected abstract SeoMetadata? GetSeoMetadata();
    protected virtual object? GetJsonLdData() => null;
}

public class SeoMetadata
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Keywords { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public string Url { get; set; } = "";
}
