# Creating Tasks & Triggers

`Tasks` are the bits of the framework which do stuff, like making a clip on twitch, writing text to a file or sending a message to chat. 

`Triggers` are the bits that notify a flow to execute, like when a chat message is sent, or every 10 seconds etc.

The two elements are quite similar for the most part but mainly they differ in how they are used, but both contain `Data`, `Task/Trigger` and `Component` aspects, all are required to function, and all are needed for them to function in the system.

## Task/Trigger Data

While they both differ in term of the interface they both represent the same notion which is purely the DATA required for a task/trigger to run, these classes will be what get persisted to the file system that describe a task.

```csharp
// Its IFlowTaskData for Tasks and IFlowTriggerData for triggers
public class WriteToLogTaskData : IFlowTaskData
{
    // This is the contract between the data class and the logic class
    public static readonly string TaskCode = "write-to-log";
    // This is the version of your data, which should increment as you change it
    public static readonly string TaskVersion = "1.0.0";
    
    // The unique id for the INSTANCE of the task data
    public Guid Id { get; set; } = Guid.NewGuid();
    // This is the code but at instance level
    public string Code => TaskCode;
    // This is the instances version (could be different to the static version)
    public string Version { get; set; } = TaskVersion;
    
    // These are the properties you want for your task logic to use
    public string Text { get; set; }
}
```

As you can see there isnt much too it, the `TaskCode` and `TaskVersion` are static because we reference them within the relevant task or trigger, the id, code, version instance properties are there to describe the task/trigger in the system, then finally the last section (the `Text` property in this example) can contain any fields of data you need to persist for this to work.

> You can have as many or little properties as you need, but NO LOGIC should exist within here as that lives in the ACTUAL task/trigger, ultimately anything in here just gets serialized into json into the flow data file.

## Creating Task Logic

```csharp
// We inherit from FlowTask<TaskDataTypeClass> for tasks
public class WriteToLogTask : FlowTask<WriteToLogTaskData>
{
    // As you can see we reference the static property on the task data class
    public override string Code => WriteToLogTaskData.TaskCode;
    // Same with version
    public override string Version => WriteToLogTaskData.TaskVersion;
    
    // This is what should be shown on the task name in the UI
    public override string Name => "Write To Log";
    // This is the category grouping it should have in the UI
    public override string Category => "Utility";
    // This is the tooltip text that will come up in the UI
    public override string Description => "Writes the text out to the log file, useful for debugging";

    // By default you will get some dependencies injected for free from the base class, but you can
    // add anything else you want to the constructor as long as its been registered in the DI configuration
    public WriteToLogTask(ILogger<FlowTask<WriteToLogTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    // This indicates if the task can be run, i.e for twitch you may not have a 
    // valid oauth token, or valid scopes for the given task, so it shouldnt run
    public override bool CanExecute() => true;
    
    // This is the core execution part where you do the real work, you are given the data instance
    // and the variables for the current flow, you should return a true for success or false for failure
    public override async Task<bool> Execute(WriteToLogTaskData data, IVariables flowVars)
    {
        var processedText = FlowStringProcessor.Process(data.Text, flowVars);
        Logger.Information(processedText);
        return true;
    }
}
```

While there is a bit of stuff there its not that complex really, you just put whatever you want to do into the `Execute` method and whenever a flow uses this task it will run that execute method with its given flow state.

> There will only ever be one instance of the task in the app at once, so each flow will call `Execute` with its own state, so keep that in mind that ALL state needed to execute needs to be included in the `data` object or `flow` variables.

## Creating Trigger Logic

```csharp
// Same as task really, we just inherit from FlowTrigger<TriggerDataObjectType>
public class OnEventRaisedTrigger : FlowTrigger<OnEventRaisedTriggerData>
{
    // The code from the trigger data class
    public override string Code => OnEventRaisedTriggerData.TriggerCode;
    // Same with version
    public override string Version => OnEventRaisedTriggerData.TriggerVersion;

    // Any variables this trigger should expose (can also be added to tasks too)
    public static VariableEntry EventDataVariable = new("event-data");
    public override VariableDescriptor[] VariableOutputs { get; } = new[] { EventDataVariable.ToDescriptor() };

    // The name of the trigger to show in the UI
    public override string Name => "On Event Raised";
    // The category the trigger should be in on the UI
    public override string Category => "Utility";
    // The tooltip to show in the UI
    public override string Description => "Triggers when the matching event is raised";

    // Much like tasks you get a lot of dependencies for free
    public OnEventRaisedTrigger(ILogger<FlowTrigger<OnEventRaisedTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    // Like tasks indicates if this trigger should be runnable
    public override bool CanExecute() => true;

    // Returns an observable with the variables for this flow, takes in the data object for state
    public override IObservable<IVariables> Execute(OnEventRaisedTriggerData data)
    {
        return EventBus.Receive<UserDataEvent>()
            .Where(x => x.EventName == data.EventName)
            .Select(x =>
            {
                var newVariables = new Variables();
                newVariables.Set(EventDataVariable, x.Data);
                return newVariables;
            });
    }
}
```

As you can see its VERY similar to a task, its just our `Execute` returns an `IObservable` which contains the variables for the flow.

> If you are unsure what an `IObservable` is, go look into Reactive Extensions online.

## Task/Trigger Components

```html
// Inherit from the CustomTaskComponent (or CustomTriggerComponent) with the data object type for the generic 
@inherits CustomTaskComponent<Strem.Flows.Default.Flows.Tasks.Utility.WriteToLogTaskData>
    
// Put whatever you want in here, its what the user will see inside the Task/Trigger section
<div class="field">
  <label class="label">Text To Log</label>
  <div class="control">
    <input class="input" type="text" placeholder="i.e Something Happened With Value V(someVar)" @bind="Data.Text">
  </div>
</div>
    
// Have any custom logic for your component in here, the mandatory Title is what is shown in the UI
@code {
    public override string Title => "Write Text To Logs";
}
```

These are basically just Blazor components inheriting from an abstract class, this keeps things consistent in terms of what data and properties will be expected, it also helps the app work out what components need to be hosted within the app.

> You cannot provide any `[Parameter]` properties to task/trigger components as they are loaded dynamically at runtime and get their data through the abstract class initialization. This being said you can `@inject ISomeDependency Dependency` as much as you want to access other objects registered within the DI configuration.