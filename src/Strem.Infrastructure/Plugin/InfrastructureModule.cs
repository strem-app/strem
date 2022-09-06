using System.Text;
using InputSimulatorStandard;
using LiteDB;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Persistity.Serializers.Json;
using Serilog;
using Strem.Core.Browsers.File;
using Strem.Core.Browsers.Web;
using Strem.Core.Components.Elements.Drag;
using Strem.Core.Events.Broker;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Executors.Logging;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Registries.Integrations;
using Strem.Core.Flows.Registries.Menus;
using Strem.Core.Flows.Registries.Tasks;
using Strem.Core.Flows.Registries.Triggers;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Core.Threading;
using Strem.Core.Utils;
using Strem.Core.Validation;
using Strem.Core.Variables;
using Strem.Data.Types;
using Strem.Infrastructure.Services;
using Strem.Infrastructure.Services.Api;
using Strem.Infrastructure.Services.Persistence;
using JsonSerializer = Persistity.Serializers.Json.JsonSerializer;

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
                new VariableDictionaryConvertor(), 
                new FlowTaskDataConvertor(), new FlowTriggerDataConvertor()
            },
            //TypeNameHandling = TypeNameHandling.Auto,
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
        services.AddSingleton<IFlowStore>(LoadFlowStore);
        
        // Flows
        services.AddSingleton<IFlowStringProcessor, FlowStringProcessor>();
        services.AddSingleton<ICommandStringProcessor, CommandStringProcessor>();
        services.AddSingleton<ITaskRegistry, TaskRegistry>();
        services.AddSingleton<ITriggerRegistry, TriggerRegistry>();
        services.AddSingleton<IIntegrationRegistry, IntegrationRegistry>();
        services.AddSingleton<IMenuRegistry, MenuRegistry>();
        
        // Execution
        services.AddSingleton<ITaskExecutor, TaskExecutor>();
        services.AddSingleton<ITriggerExecutor, TriggerExecutor>();
        services.AddSingleton<IFlowExecutionLogger, FlowExecutionLogger>();
        services.AddSingleton<IFlowExecutionEngine, IFlowExecutor, FlowExecutionEngine>();
        
        // OS Specific
        services.AddSingleton<IInputSimulator, InputSimulator>();
        services.AddSingleton<IFileBrowser, FileBrowser>();
        
        // UI
        services.AddSingleton<DragController>();
        
        // Plugin (this isnt technically a plugin I know)
        services.AddSingleton<IPluginStartup, InfrastructurePluginStartup>();
    }
    
    public IAppState LoadAppState(IServiceProvider services)
    {
        var stateFileHandler = services.GetService<IAppFileHandler>();
        return Task.Run(async () => await stateFileHandler.LoadAppState()).Result;
    }
    
    public IFlowStore LoadFlowStore(IServiceProvider services)
    {
        var stateFileHandler = services.GetService<IAppFileHandler>();
        return Task.Run(async () => await stateFileHandler.LoadFlowStore()).Result;
    }

    public void SetupDatabase(IServiceCollection services)
    {
        if (!Directory.Exists(PathHelper.StremDataDirectory))
        { Directory.CreateDirectory(PathHelper.StremDataDirectory); }
        
        var profile = "default";
        var dbPath = $"{PathHelper.StremDataDirectory}/{profile}.db";
        services.AddSingleton<ILiteDatabase>(x => new LiteDatabase(dbPath));
        services.AddSingleton<IFlowRepository, FlowRepository>();
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