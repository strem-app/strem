using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Strem.Core.Components.Elements.Drag;
using Strem.Core.Extensions;
using Strem.Core.Services.UI.Animation;
using Strem.Core.Services.UI.Modal;
using Strem.Core.Services.UI.Notifications;
using Strem.Infrastructure.ActionFilters;

namespace Strem.Infrastructure.Services.Api;

public class InternalWebHost : IInternalWebHost
{
    public WebApplication Host { get; protected set; }
    public bool IsRunning { get; set; }
    
    public (Assembly assembly, IRequiresApiHostingModule module)[] GetAllApiHostingPlugins()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        return assemblies.SelectMany(x => x.GetTypes().WhereClassesImplement(typeof(IRequiresApiHostingModule)))
            .Select(x => (x.Assembly, (IRequiresApiHostingModule)Activator.CreateInstance(x)))
            .ToArray();
    }

    public WebApplication CreateApplication()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddRouting();
        builder.Services.AddRazorPages();
#if DEBUG
        builder.Services.AddServerSideBlazor().AddCircuitOptions(x => x.DetailedErrors = true);
#else
        builder.Services.AddServerSideBlazor();
#endif
        builder.Services.AddServerSideBlazor();
        builder.Services.AddControllersWithViews(x =>
        {
            x.Filters.Add<LogActionFilter>();
        });
        
        var mvcBuilder = builder.Services.AddMvcCore();
        mvcBuilder.AddApplicationPart(GetType().Assembly);
        
        var pluginModules = GetAllApiHostingPlugins();
        foreach (var pluginModule in pluginModules)
        { mvcBuilder.AddApplicationPart(pluginModule.assembly); }

        RegisterIsolatedServices(builder.Services);

        if(!builder.Environment.IsDevelopment())
        { builder.Logging.ClearProviders(); }
        
        return builder.Build();
    }

    private void RegisterIsolatedServices(IServiceCollection services)
    {
        services.AddTransient<INotifier, Notifier>();
        services.AddTransient<IAnimator, Animator>();
        services.AddTransient<IModalService, ModalService>();
        services.AddTransient<IDragAndDropService, DragAndDropService>();
    }

    public void StartHost()
    {
        var app = CreateApplication();
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseRouting();
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            ServeUnknownFileTypes = true
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            RequestPath = new PathString("/Plugins"),
            FileProvider = new PhysicalFileProvider(StremPathHelper.PluginPath),
            ServeUnknownFileTypes = true
        });
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();
            endpoints.MapBlazorHub();
        });
        
        app.RunAsync($"http://*:{InternalWebHostConfiguration.ApiHostPort}");
        IsRunning = true;
        Host = app;
    }

    public void StopHost()
    {
        Host.DisposeAsync();
        IsRunning = false;
    }
}