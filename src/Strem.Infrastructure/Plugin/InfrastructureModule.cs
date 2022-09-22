using System.Text;
using LiteDB;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Persistity.Serializers.Json;
using Serilog;
using Strem.Core.Components.Elements.Drag;
using Strem.Core.Events.Broker;
using Strem.Core.Events.Bus;
using Strem.Core.Plugins;
using Strem.Core.Services.Browsers.Web;
using Strem.Core.Services.Registries.Integrations;
using Strem.Core.Services.Registries.Menus;
using Strem.Core.Services.Threading;
using Strem.Core.Services.UI.Animation;
using Strem.Core.Services.UI.Notifications;
using Strem.Core.Services.Utils;
using Strem.Core.Services.Validation;
using Strem.Core.State;
using Strem.Data.Types;
using Strem.Flows.Variables;
using Strem.Infrastructure.Services;
using Strem.Infrastructure.Services.Api;
using Strem.Infrastructure.Services.Persistence;
using JsonSerializer = Persistity.Serializers.Json.JsonSerializer;
using VariableEntryConvertor = Strem.Core.Variables.VariableEntryConvertor;

namespace Strem.Infrastructure.Plugin;

public class InfrastructureModule : IRequiresApiHostingModule
{
    public void Setup(IServiceCollection services)
    {
        // Tertiary
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new VariableEntryConvertor(), 
                new FlowTaskDataConvertor(), new FlowTriggerDataConvertor()
            },
            Formatting = Formatting.Indented
        };
        
        // Logging
        services.AddLogging(x => x.AddSerilog(SetupLogger()));
        services.AddSingleton<IErrorBoundaryLogger, ErrorBoundryLogger>();
        
        // General
        services.AddSingleton<IMessageBroker, MessageBroker>();
        services.AddSingleton<IThreadHandler, ThreadHandler>();
        services.AddSingleton<IEventBus, EventBus>();
        services.AddSingleton<IRandomizer>(new DefaultRandomizer(new Random()));
        services.AddSingleton<IWebBrowser, WebBrowser>();
        services.AddSingleton<ICloner, Cloner>();
        services.AddSingleton<IDataValidator, DataValidator>();
        
        // UI
        services.AddTransient<INotifier, Notifier>();
        services.AddTransient<IAnimator, Animator>();
        services.AddSingleton<DragController>();
        
        // Hosting
        services.AddSingleton<IInternalWebHost, InternalWebHost>();
        services.AddSingleton(SetupLogger());
        
        // Encryption
        // TODO: this needs to be moved somewhere else at some point
        var key = Encoding.UTF8.GetBytes("UxRBN8hfjzTG86d6SkSSNzyUhERGu5Zj");
        var iv = Encoding.UTF8.GetBytes("7cA8jkRMJGZ8iMeJ");
        services.AddSingleton<IEncryptor>(new CustomEncryptor(key, iv));
        
        // Persistence Base
        services.AddSingleton<ISerializer, JsonSerializer>();
        services.AddSingleton<IDeserializer, JsonDeserializer>();
        services.AddSingleton<PipelineBuilder>();

        // DB
        SetupDatabase(services);

        // State/Stores
        services.AddSingleton<IAppFileHandler, AppFileHandler>();
        services.AddSingleton<IAppState>(LoadAppState);

        // Registries
        services.AddSingleton<IIntegrationRegistry, IntegrationRegistry>();
        services.AddSingleton<IMenuRegistry, MenuRegistry>();

        // Plugin (this isnt technically a plugin I know)
        services.AddSingleton<IPluginStartup, InfrastructurePluginStartup>();
    }
    
    public IAppState LoadAppState(IServiceProvider services)
    {
        var stateFileHandler = services.GetService<IAppFileHandler>();
        return Task.Run(async () => await stateFileHandler.LoadAppState()).Result;
    }

    public void SetupDatabase(IServiceCollection services)
    {
        if (!Directory.Exists(StremPathHelper.StremDataDirectory))
        { Directory.CreateDirectory(StremPathHelper.StremDataDirectory); }
        
        var profile = "default";
        var dbPath = $"{StremPathHelper.StremDataDirectory}/{profile}.db";
        services.AddSingleton<ILiteDatabase>(x => new LiteDatabase(dbPath));
        services.AddSingleton<IAppVariablesRepository, AppVariablesRepository>();
        services.AddSingleton<IUserVariablesRepository, UserVariablesRepository>();
    }
    
    public Serilog.ILogger SetupLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/strem.log", rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] ({SourceContext}.{Method}) {Message}{NewLine}{Exception}")
            .Enrich.FromLogContext()
            
            .CreateLogger();
    }
}