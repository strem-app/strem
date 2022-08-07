using System.Text.RegularExpressions;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Core.Flows.Processors;

public class FlowStringProcessor : IFlowStringProcessor
{
    public IAppState AppState { get; set; }
    public Regex ReplacePattern = new(@"(V\(([\w\s\.,\-_\:\|]*)\))");

    public FlowStringProcessor(IAppState appState)
    {
        AppState = appState;
    }

    public string GetVariable(IVariables flowVariables, string name, string context = VariableEntry.DefaultContext)
    {
        if (flowVariables.Has(name, context)) { return flowVariables.Get(name, context); }
        if (AppState.UserVariables.Has(name, context)) { return AppState.UserVariables.Get(name, context); }
        if (AppState.TransientVariables.Has(name, context)) { return AppState.TransientVariables.Get(name, context); }
        return string.Empty;
    }
    
    public string VariableMatchEvaluator(Match x, IVariables flowVariables)
    {
        if (x.Groups.Count < 3) { return x.Value; }
        var matchingGroup = x.Groups[2].Value;
        var sections = matchingGroup.Split(",");
        return sections.Length switch
        {
            1 => GetVariable(flowVariables, sections[0].Trim()),
            2 => GetVariable(flowVariables, sections[0].Trim(), sections[1].Trim()),
            _ => x.Value
        };
    }
    
    public string Process(string textToProcess, IVariables flowVariables)
    {
        var processor = new MatchEvaluator(x => VariableMatchEvaluator(x, flowVariables));
        
        var output = string.Empty;
        var lastOutput = textToProcess;
        var iterations = 0;
        while (iterations < 5)
        {
            output = ReplacePattern.Replace(lastOutput, processor);
            
            if (output == lastOutput)
            { break; }

            lastOutput = output;
            iterations++;
        }
        
        return output;
    }
}