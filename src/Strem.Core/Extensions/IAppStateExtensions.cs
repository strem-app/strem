using Strem.Core.State;
using Strem.Core.Types;
using Strem.Core.Variables;

namespace Strem.Core.Extensions;

public static class IAppStateExtensions
{
    public static string GetVariable(this IAppState appState, IVariables flowVars, string name, string context = VariableEntry.DefaultContext)
    {
        if (flowVars.Has(name, context)) { return flowVars.Get(name, context); }
        if (appState.UserVariables.Has(name, context)) { return appState.UserVariables.Get(name, context); }
        if (appState.TransientVariables.Has(name, context)) { return appState.TransientVariables.Get(name, context); }
        return string.Empty;
    }
    
    public static string GetVariable(this IAppState appState, IVariables flowVars, VariableScopeType scope, string name, string context = VariableEntry.DefaultContext)
    {
        switch (scope)
        {
            case VariableScopeType.Application: appState.TransientVariables.Get(name, context); break;
            case VariableScopeType.Flow: flowVars.Get(name, context); break;
            default: appState.UserVariables.Get(name, context); break;
        }
        return string.Empty;
    }
    
    public static void SetVariable(this IAppState appState, IVariables flowVars, VariableScopeType scope, string name, string context, string value)
    {
        switch (scope)
        {
            case VariableScopeType.Application: appState.TransientVariables.Set(name, context, value); break;
            case VariableScopeType.Flow: flowVars.Set(name, context, value); break;
            default: appState.UserVariables.Set(name, context, value); break;
        }
    }
    
    public static void SetVariable(this IAppState appState, IVariables flowVars, VariableScopeType scope, string name, string value)
    {
        switch (scope)
        {
            case VariableScopeType.Application: appState.TransientVariables.Set(name, value); break;
            case VariableScopeType.Flow: flowVars.Set(name, value); break;
            default: appState.UserVariables.Set(name, value); break;
        }
    }
    
    public static void SetVariable(this IAppState appState, IVariables flowVars, string name, string context, string value)
    {
        if (flowVars.Has(name, context)) { flowVars.Set(name, context, value); }
        if (appState.UserVariables.Has(name, context)) { appState.UserVariables.Set(name, context, value); }
        if (appState.TransientVariables.Has(name, context)) { appState.TransientVariables.Set(name, context, value); }
    }
}