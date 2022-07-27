using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Persistity.Serializers.Json;
using Serilog;
using Strem.Core.DI;
using Strem.Core.Events;
using Strem.Core.Events.Broker;
using Strem.Core.State;
using Strem.Core.Threading;
using Strem.Infrastructure.Services.Api;
using Strem.Infrastructure.Services.Persistence;
using Strem.Infrastructure.Services.Persistence.App;
using Strem.Infrastructure.Services.Persistence.User;
using Strem.Infrastructure.Services.Web;
using ILogger = Serilog.ILogger;

namespace Strem.Infrastructure.Modules;

public class InfrastructureModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // General
        services.AddSingleton<IMessageBroker, MessageBroker>();
        services.AddSingleton<IThreadHandler, ThreadHandler>();
        services.AddSingleton<IEventBus, EventBus>();
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
        services.AddSingleton<StateFileCreator>();
        services.AddSingleton<IAppState>(x => CreateAppState(x).GetAwaiter().GetResult());
    }
    
    public ILogger SetupLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/strem.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    public async Task<AppState> CreateAppState(IServiceProvider services)
    {
        var appVariableLoader = services.GetService<ILoadAppVariablesPipeline>();
        var userVariableLoader = services.GetService<ILoadUserVariablesPipeline>();
        
        var appVariables = appVariableLoader.Execute().GetAwaiter().GetResult();
        var userVariables = userVariableLoader.Execute().GetAwaiter().GetResult();

        return new AppState(
            userVariables ?? new Variables(),
            appVariables ?? new Variables(),
            new Variables()
        );
    }
}