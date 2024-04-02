using System.Text;
using LiteDB;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Persistity.Serializers.Json;
using Serilog;
using SharpHook;
using SharpHook.Reactive;
using Strem.Core.Components.Elements.Drag;
using Strem.Core.Events.Broker;
using Strem.Core.Events.Bus;
using Strem.Core.Plugins;
using Strem.Core.Services.Browsers.Web;
using Strem.Core.Services.Execution;
using Strem.Core.Services.Input;
using Strem.Core.Services.Registries.Integrations;
using Strem.Core.Services.Registries.Menus;
using Strem.Core.Services.Threading;
using Strem.Core.Services.UI.Animation;
using Strem.Core.Services.UI.Modal;
using Strem.Core.Services.UI.Notifications;
using Strem.Core.Services.Utils;
using Strem.Core.Services.Validation;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Data.Types;
using Strem.Flows.Variables;
using Strem.Infrastructure.Extensions;
using Strem.Infrastructure.Services;
using Strem.Infrastructure.Services.Api;
using JsonSerializer = Persistity.Serializers.Json.JsonSerializer;
using VariableEntryConvertor = Strem.Core.Variables.VariableEntryConvertor;

namespace Strem.Infrastructure.Plugin;

public class InfrastructureModule : IRequiresApiHostingModule
{
    public string[] RequiredConfigurationKeys { get; } = new[]
    {
        InfrastructurePluginSettings.EncryptionKeyKey,
        InfrastructurePluginSettings.EncryptionIVKey
    };
    
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
        services.AddSingleton<IExclusiveExecutionHandler, ExclusiveExecutionHandler>();
        services.AddSingleton<IEventBus, EventBus>();
        services.AddSingleton<IRandomizer>(new DefaultRandomizer(new Random()));
        services.AddSingleton<IWebBrowser, WebBrowser>();
        services.AddSingleton<ICloner, Cloner>();
        services.AddSingleton<IDataValidator, DataValidator>();
        
        // Input
        services.AddSingleton<IReactiveGlobalHook>(_ => new SimpleReactiveGlobalHook(GlobalHookType.Keyboard));
        services.AddSingleton<IEventSimulator, EventSimulator>();
        services.AddSingleton<IInputHandler, DefaultInputHandler>();
        
        // UI
        services.AddTransient<INotifier, Notifier>();
        services.AddTransient<IAnimator, Animator>();
        services.AddTransient<IModalService, ModalService>();
        services.AddTransient<IDragAndDropService, DragAndDropService>();
        
        // Hosting
        services.AddSingleton<IInternalWebHost, InternalWebHost>();
        services.AddSingleton(SetupLogger());
        
        // Encryption
        services.AddSingleton<IEncryptor>(SetupEncryption);
        
        // Persistence Base
        services.AddSingleton<ISerializer, JsonSerializer>();
        services.AddSingleton<IDeserializer, JsonDeserializer>();
        services.AddSingleton<PipelineBuilder>();

        // DB
        SetupDatabase(services);

        // State/Stores
        services.AddSingleton(LoadAppState);
        
        // Registries
        services.AddSingleton<IIntegrationRegistry, IntegrationRegistry>();
        services.AddSingleton<IMenuRegistry, MenuRegistry>();

        // Plugin (this isnt technically a plugin I know)
        services.AddSingleton<IPluginStartup, InfrastructurePluginStartup>();
    }

    public IEncryptor SetupEncryption(IServiceProvider services)
    {
        var appConfig = services.GetService<IApplicationConfig>();
        var key = Encoding.UTF8.GetBytes(appConfig.GetEncryptionKey());
        var iv = Encoding.UTF8.GetBytes(appConfig.GetEncryptionIV());
        return new CustomEncryptor(key, iv);
    }
    
    public IAppState LoadAppState(IServiceProvider services)
    {
        var appVariablesRepo = services.GetService<IAppVariablesRepository>();
        var appVariableData = new Dictionary<VariableEntry, string>(appVariablesRepo?.GetAll() ?? Array.Empty<KeyValuePair<VariableEntry, string>>());
        var appVariables = new Variables(appVariableData);
        
        var userVariablesRepo = services.GetService<IUserVariablesRepository>();
        var userVariableData = new Dictionary<VariableEntry, string>(userVariablesRepo?.GetAll() ?? Array.Empty<KeyValuePair<VariableEntry, string>>());
        var userVariables = new Variables(new Dictionary<VariableEntry, string>(userVariableData));
        return new AppState(userVariables, appVariables, new Variables());
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
            .WriteTo.File($"{StremPathHelper.LogPath}/strem.log", rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] ({SourceContext}.{Method}) {Message}{NewLine}{Exception}")
            .Enrich.FromLogContext()
            
            .CreateLogger();
    }
}