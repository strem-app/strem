using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Strem.Core.Events.Bus;
using Strem.Core.Extensions;
using Strem.Core.Flows.Processors;
using Strem.Core.Flows.Tasks;
using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;

namespace Strem.Flows.Default.Flows.Tasks.Variables;

public class RegexToVariableTask : FlowTask<RegexToVariableTaskData>
{
    public override string Code => RegexToVariableTaskData.TaskCode;
    public override string Version => RegexToVariableTaskData.TaskVersion;
    
    public override string Name => "Regex To Variable";
    public override string Category => "Variables";
    public override string Description => "Uses regex to extract a portion of the source to a new variable";

    public RegexToVariableTask(ILogger<FlowTask<RegexToVariableTaskData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus) : base(logger, flowStringProcessor, appState, eventBus)
    {
    }

    public override bool CanExecute() => true;

    public override async Task Execute(RegexToVariableTaskData data, IVariables flowVars)
    {
        var processedSource = FlowStringProcessor.Process(data.Source, flowVars);
        var processedName = FlowStringProcessor.Process(data.Name, flowVars);
        var processedContext = FlowStringProcessor.Process(data.Context, flowVars);

        var matchedText = string.Empty;
        
        try
        {
            var match = Regex.Match(processedSource, data.Pattern);
            matchedText = match.Groups[1].Value;
        }
        catch
        {
            // ignored
        }

        switch (data.Scope)
        {
            case VariableScope.Application: AppState.TransientVariables.Set(processedName, processedContext, matchedText); break;
            case VariableScope.Flow: flowVars.Set(processedName, processedContext, matchedText); break;
            default: AppState.UserVariables.Set(processedName, processedContext, matchedText); break;
        }
    }
}