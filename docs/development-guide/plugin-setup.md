# Plugin Setup

The application has been built to be easy to add your own tasks/triggers through either adding to existing source projects or making your own projects and loading them at runtime.

There are 3 main parts that you can extend, which are `Tasks`, `Triggers` and `Integrations`, however there is also a couple of other important bits worth covering first, which tell the application how to load things.

> The whole of the application uses `Dependency Injection` throughout so you can load in a plugin that swaps out implementations of internals if you wanted to really alter stuff.

## `IDependencyModule` and `IPluginStartup`

So this is a boring bit but is very important, as the application uses 2 main avenues to setup your plugin/projects to run, and we do this internally too so its an approach used throughout all projects within the framework.

>  The first one `IDependencyModule` describes to the app what dependencies you want to expose and register while the second acts like the initialization point of the plugin at runtime.

### Example Dependency Module

```csharp
public class DummyModule : IDependencyModule
{
    public void Setup(IServiceCollection services)
    {
        // Register any dependencies you need in your code base
        services.AddSingleton<ISomething, Something>();
        
        // This extension method will register any task/trigger components within your assembly for the app to host
        var thisAssembly = GetType().Assembly;
        services.RegisterAllTasksAndComponentsIn(thisAssembly);
        services.RegisterAllTriggersAndComponentsIn(thisAssembly);
    }
}
```

When `Strem` boots up it will look through any assemblies loaded (or any in `Plugins` folder) for any implementations of `IDependencyModule` and will register them automatically.

> There is also `IRequiresApiHostingModule` which can be used instead, however this one indicates you also want to host MVC content within the application on its internal web server, currently this is used for OAuth but will probably also be used for other fancy things going forward.


### Example Plugin Startup
```csharp
public class SomePluginStartup : IPluginStartup, IDisposable
{
    private CompositeDisposable _subs = new();
    
    public IEventBus EventBus { get; }
    public IAppState AppState { get; }
    public ILogger<OBSPluginStartup> Logger { get; }

    public SomePluginStartup(IEventBus eventBus, IAppState appState, ILogger<OBSPluginStartup> logger)
    {
        EventBus = eventBus;
        AppState = appState;
        Logger = logger;
    }

    public async Task StartPlugin()
    {
        EventBus.Receive<SomeEvent>()
            .Subscribe(x => Logger.Error($"Something Happened"))
            .AddTo(_subs);
    }

    public void Dispose()
    { _subs?.Dispose(); }
}
```

As you can see the `StartPlugin` method is the entry point where you can subscribe to anything or start any custom logic/setup.

> For example the `Twitch Integration` will attempt to start a connection to twitch chat if you have available credentials, and also handle re-validation of oauth and other things,

The plugin startup classes are also auto started within the app when it runs, so if you have any startup logic in your assembly just plop it in there.