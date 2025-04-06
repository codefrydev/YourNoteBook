using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using YourNoteBook.Data;
using YourNoteBook.Helper;
using YourNoteBook.Models;

namespace YourNoteBook.Services.WeServices;

public static  class AddServices
{
    public static void ScopedServiceAdder(this WebAssemblyHostBuilder  builder)
    { 
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped<IFirebaseJsInteropService, FirebaseJsInteropService>();
        builder.Services.AddScoped<IFirebaseHelper, FirebaseHelper>(); 
        builder.Services.AddScoped<IManager<FolderModel>, FolderManger>(); 
        builder.Services.AddScoped<IManager<Shortcut>, ShortcutManager>(); 
        builder.Services.AddScoped<IManager<Note>, NotesManager>();
    }
}