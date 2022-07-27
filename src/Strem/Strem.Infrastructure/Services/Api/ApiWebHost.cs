using Strem.Core.Extensions;
using Strem.Infrastructure.ActionFilters;

namespace Strem.Infrastructure.Services.Api;

public class ApiWebHost : IApiWebHost
{
    public static readonly int InternalPort = 56721;
    
    public WebApplication Host { get; protected set; }

    public WebApplication CreateApplication(ApiHostConfiguration configuration = null)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddRouting();
        builder.Services.AddControllers(x =>
        {
            x.Filters.Add<LogActionFilter>();
        });

        if (configuration?.Modules != null)
        {
            foreach (var module in configuration.Modules)
            { builder.Services.AddModule(module); }
        }
        
        var mvcBuilder = builder.Services.AddMvcCore();
        mvcBuilder.AddApplicationPart(GetType().Assembly);
        
        if (configuration?.ControllerAssemblies != null)
        {
            foreach (var assembly in configuration.ControllerAssemblies)
            { mvcBuilder.AddApplicationPart(assembly); }
        }
        
        return builder.Build();
    }
    
    public void StartHost(ApiHostConfiguration configuration = null)
    {
        var app = CreateApplication(configuration);
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.RunAsync($"http://localhost:{InternalPort}");

        Host = app;
    }

    public void StopHost()
    {
        Host.DisposeAsync();
    }
}