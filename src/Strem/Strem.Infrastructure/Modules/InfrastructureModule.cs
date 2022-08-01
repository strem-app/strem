using Newtonsoft.Json;
using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Persistity.Serializers.Json;
using Serilog;
using Strem.Core.DI;
using Strem.Core.Events;
using Strem.Core.Events.Broker;
using Strem.Core.Flows;
using Strem.Core.Flows.Processors;
using Strem.Core.Plugins;
using Strem.Core.State;
using Strem.Core.Threading;
using Strem.Core.Utils;
using Strem.Core.Variables;
using Strem.Infrastructure.Plugin;
using Strem.Infrastructure.Services.Api;
using Strem.Infrastructure.Services.Persistence;
using Strem.Infrastructure.Services.Persistence.App;
using Strem.Infrastructure.Services.Persistence.User;
using Strem.Infrastructure.Services.Web;
using JsonSerializer = Persistity.Serializers.Json.JsonSerializer;

namespace Strem.Infrastructure.Modules;

public class InfrastructureModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Tertiary
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new VariableDictionaryConvertor() },
            Formatting = Formatting.Indented
        };
        
        // Logging
        services.AddLogging(x => x.AddSerilog(SetupLogger()));
        
        // General
        services.AddSingleton<IMessageBroker, MessageBroker>();
        services.AddSingleton<IThreadHandler, ThreadHandler>();
        services.AddSingleton<IEventBus, EventBus>();
        services.AddSingleton<IRandomizer>(new DefaultRandomizer(new Random()));
        services.AddSingleton<IWebLoader, WebLoader>();
        
        // Hosting
        services.AddSingleton<IApiWebHost, ApiWebHost>();
        services.AddSingleton(SetupLogger());
        
        // Persistence Base
        services.AddSingleton<IEncryptor>(new AesEncryptor("super-secret"));
        services.AddSingleton<ISerializer, JsonSerializer>();
        services.AddSingleton<IDeserializer, JsonDeserializer>();
        services.AddSingleton<PipelineBuilder>();
        
        // Persistence
        services.AddSingleton<ISaveAppVariablesPipeline, SaveAppVariablesPipeline>();
        services.AddSingleton<ILoadAppVariablesPipeline, LoadAppVariablesPipeline>();
        services.AddSingleton<ISaveUserVariablesPipeline, SaveUserVariablesPipeline>();
        services.AddSingleton<ILoadUserVariablesPipeline, LoadUserVariablesPipeline>();

        // State
        services.AddSingleton<IStateFileHandler, StateFileHandler>();
        services.AddSingleton<IAppState>(LoadAppState);
        
        // Flows
        services.AddSingleton<IFlowStringProcessor, FlowStringProcessor>();
        
        // Plugin (this isnt technically a plugin I know)
        services.AddSingleton<IPluginStartup, InfrastructurePluginStartup>();
    }

    public IAppState LoadAppState(IServiceProvider services)
    {
        var stateFileHandler = services.GetService<IStateFileHandler>();
        return Task.Run(async () => await stateFileHandler.LoadAppState()).Result;
    }
    
    public Serilog.ILogger SetupLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/strem.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}