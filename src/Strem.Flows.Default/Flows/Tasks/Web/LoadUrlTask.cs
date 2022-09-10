using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Flows.Data.Tasks;
using Strem.Core.Services.Browsers.Web;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Executors;
using Strem.Flows.Processors;

namespace Strem.Flows.Default.Flows.Tasks.Web;

public class LoadUrlTask : FlowTask<LoadUrlTaskData>
{
    public override string Code => LoadUrlTaskData.TaskCode;
    public override string Version => LoadUrlTaskData.TaskVersion;
    
    public override string Name => "Load A Url";
    public override string Category => "Web";
    public override string Description => "Loads a url in a browser";
    
    public IWebBrowser WebBrowser { get; }

    public LoadUrlTask(ILogger<FlowTask<LoadUrlTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IWebBrowser webBrowser) : base(logger, flowStringProcessor, appState, eventBus)
    {
        WebBrowser = webBrowser;
    }

    public override bool CanExecute() => true;
    
    public override async Task<ExecutionResult> Execute(LoadUrlTaskData data, IVariables flowVars)
    {
        var processedUrl = FlowStringProcessor.Process(data.Url, flowVars);
        WebBrowser.LoadUrl(processedUrl);
        return ExecutionResult.Success();
    }
}