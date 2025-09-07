using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blazored.LocalStorage;
using YourNoteBook.Core.Interfaces;
using YourNoteBook.Core.Abstractions;
using YourNoteBook.Core.Entities;
using YourNoteBook.Infrastructure.Services.Firebase;
using YourNoteBook.Infrastructure.Services;
using YourNoteBook.Shared.Helpers;
using YourNoteBook.Shared.Services.DataManagement;
using YourNoteBook.Shared.Services.StateManagement;
using YourNoteBook.Shared.Services.Utilities;
using YourNoteBook.Shared.Utilities;
using YourNoteBook.Infrastructure.Data.Context;

namespace YourNoteBook.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring services in the dependency injection container
/// </summary>
public static class ServiceCollectionExtensions
{
    public static void ConfigureServices(this WebAssemblyHostBuilder builder)
    {
        var services = builder.Services;

        // Add Blazored Local Storage
        services.AddBlazoredLocalStorage();

        // Add HTTP Client
        services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        // Add Firebase service
        services.AddScoped<IFirebaseJsInteropService, FirebaseJsInteropService>();
        services.AddScoped<IFirebaseHelper, FirebaseHelper>();

        // Add manager factory
        services.AddScoped<IManagerFactory, ManagerFactory>();

        // Add data managers
        services.AddScoped<IManager<Note>, NotesManager>();
        services.AddScoped<IManager<Folder>, FoldersManager>();
        services.AddScoped<IManager<Shortcut>, ShortcutsManager>();
        services.AddScoped<IManager<Tag>, TagsManager>();
        services.AddScoped<IManager<Category>, CategoriesManager>();

        // Add in-memory repository
        services.AddSingleton<InMemoryRepo>();

        // Add global data service
        services.AddScoped<IGlobalDataService, GlobalDataService>();

        // Add state management
        services.AddScoped<StateManager>();

        // Add logger service
        services.AddScoped<ILoggerService, LoggerService>();

        // Add snackbar service
        services.AddSingleton<SnackbarService>();
    }
}

/// <summary>
/// Manager for Notes
/// </summary>
public class NotesManager : BaseManager<Note>
{
    public NotesManager(IFirebaseJsInteropService firebaseService) 
        : base(firebaseService, "notes") { }
}

/// <summary>
/// Manager for Folders
/// </summary>
public class FoldersManager : BaseManager<Folder>
{
    public FoldersManager(IFirebaseJsInteropService firebaseService) 
        : base(firebaseService, "folders") { }
}

/// <summary>
/// Manager for Shortcuts
/// </summary>
public class ShortcutsManager : BaseManager<Shortcut>
{
    public ShortcutsManager(IFirebaseJsInteropService firebaseService) 
        : base(firebaseService, "shortcuts") { }
}

/// <summary>
/// Manager for Tags
/// </summary>
public class TagsManager : BaseManager<Tag>
{
    public TagsManager(IFirebaseJsInteropService firebaseService) 
        : base(firebaseService, "tags") { }
}

/// <summary>
/// Manager for Categories
/// </summary>
public class CategoriesManager : BaseManager<Category>
{
    public CategoriesManager(IFirebaseJsInteropService firebaseService) 
        : base(firebaseService, "categories") { }
}