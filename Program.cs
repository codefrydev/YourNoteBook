using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using YourNoteBook;
using YourNoteBook.Data;
using YourNoteBook.Helper;
using YourNoteBook.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<InMemoryRepo>();
builder.Services.AddScoped<IFirebaseJsInteropService, FirebaseJsInteropService>();
builder.Services.AddScoped<IFirebaseHelper, FirebaseHelper>(); 
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();