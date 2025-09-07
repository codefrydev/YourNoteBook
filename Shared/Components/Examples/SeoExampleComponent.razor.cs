using Microsoft.AspNetCore.Components;
using YourNoteBook.Core.Entities;
using YourNoteBook.Shared.Services.SEO;
using YourNoteBook.Shared.Models.SEO;

namespace YourNoteBook.Shared.Components.Examples;

public partial class SeoExampleComponent : ComponentBase
{
    [Inject] private ISeoMetadataService SeoService { get; set; } = default!;
    [Inject] private IDynamicSeoService DynamicSeoService { get; set; } = default!;

    [Parameter] public Note? Note { get; set; }
    [Parameter] public Folder? Folder { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Note != null)
        {
            await SetDynamicSeoMetadataAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task SetDynamicSeoMetadataAsync()
    {
        if (Note == null) return;

        try
        {
            // Generate dynamic SEO metadata for the note
            var seoMetadata = await DynamicSeoService.GenerateNoteSeoAsync(Note, Folder);
            
            // Set all metadata types
            await SeoService.SetPageMetadataAsync(
                seoMetadata.Title,
                seoMetadata.Description,
                seoMetadata.Keywords,
                seoMetadata.ImageUrl,
                seoMetadata.Url
            );

            await SeoService.SetOpenGraphAsync(
                seoMetadata.Title,
                seoMetadata.Description,
                seoMetadata.ImageUrl,
                seoMetadata.Url
            );

            await SeoService.SetTwitterCardAsync(
                seoMetadata.Title,
                seoMetadata.Description,
                seoMetadata.ImageUrl,
                seoMetadata.Url
            );

            // Generate and set JSON-LD structured data
            var articleJsonLd = await DynamicSeoService.GenerateNoteJsonLdAsync(Note, Folder);
            await SeoService.SetJsonLdAsync(articleJsonLd);

            // Generate breadcrumb navigation
            var breadcrumbJsonLd = await DynamicSeoService.GenerateBreadcrumbAsync(
                "Note", 
                Folder?.Name, 
                Note.Title
            );
            
            // You can set multiple JSON-LD objects by combining them
            var combinedJsonLd = new
            {
                article = articleJsonLd,
                breadcrumb = breadcrumbJsonLd
            };
            
            await SeoService.SetJsonLdAsync(combinedJsonLd);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting dynamic SEO metadata: {ex.Message}");
        }
    }
}
