using System.Reflection;
using Strem.Core.Extensions;
using Strem.Infrastructure.ActionFilters;

namespace Strem.Infrastructure.Services.Api;

public class ApiWebHost : IApiWebHost
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
        builder.Services.AddControllersWithViews(x =>
        {
            x.Filters.Add<LogActionFilter>();
        });

        var pluginModules = GetAllApiHostingPlugins();
        foreach (var pluginModule in pluginModules)
        { builder.Services.AddModule(pluginModule.module); }
        
        
        var mvcBuilder = builder.Services.AddMvcCore();
        mvcBuilder.AddApplicationPart(GetType().Assembly);
        
        foreach (var pluginModule in pluginModules)
        { mvcBuilder.AddApplicationPart(pluginModule.assembly); }

        if(!builder.Environment.IsDevelopment())
        { builder.Logging.ClearProviders(); }
        
        return builder.Build();
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
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.RunAsync($"http://localhost:{ApiHostConfiguration.ApiHostPort}");
        IsRunning = true;
        Host = app;
    }

    public void StopHost()
    {
        Host.DisposeAsync();
        IsRunning = false;
    }
}