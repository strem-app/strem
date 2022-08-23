using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Flows.Executors;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;
using Strem.Core.Web;

namespace Strem.Flows.Default.Flows.Tasks.Web;

public class LoadUrlTask : FlowTask<LoadUrlTaskData>
{
    public override string Code => LoadUrlTaskData.TaskCode;
    public override string Version => LoadUrlTaskData.TaskVersion;
    
    public override string Name => "Load A Url";
    public override string Category => "Web";
    public override string Description => "Loads a url in a browser";
    
    public IBrowserLoader BrowserLoader { get; }

    public LoadUrlTask(ILogger<FlowTask<LoadUrlTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IBrowserLoader browserLoader) : base(logger, flowStringProcessor, appState, eventBus)
    {
        BrowserLoader = browserLoader;
    }

    public override bool CanExecute() => true;
    
    public override async Task<ExecutionResult> Execute(LoadUrlTaskData data, IVariables flowVars)
    {
        var processedUrl = FlowStringProcessor.Process(data.Url, flowVars);
        BrowserLoader.LoadUrl(processedUrl);
        return ExecutionResult.Success();
    }
}