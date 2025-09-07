using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using YourNoteBook;
using YourNoteBook.Infrastructure.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure all services using the centralized configuration
builder.ConfigureServices();

await builder.Build().RunAsync();