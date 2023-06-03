using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Strem.Core.Events;
using Strem.Core.Events.Bus;
using Strem.Core.Services.Input;
using Strem.Flows.Data.Triggers;
using Strem.Core.State;
using Strem.Core.Variables;
using Strem.Flows.Processors;

namespace Strem.Flows.Default.Flows.Triggers.Input;

public class OnKeyboardShortcutTrigger : FlowTrigger<OnKeyboardShortcutTriggerData>
{
    public override string Code => OnKeyboardShortcutTriggerData.TriggerCode;
    public override string Version => OnKeyboardShortcutTriggerData.TriggerVersion;

    public override string Name => "On Keyboard Shortcut";
    public override string Category => "Input";
    public override string Description => "Triggers when a shortcut is entered globally";

    public IInputHandler InputHandler { get; }
    
    public OnKeyboardShortcutTrigger(ILogger<FlowTrigger<OnKeyboardShortcutTriggerData>> logger, IFlowStringProcessor flowStringProcessor, IAppState appState, IEventBus eventBus, IInputHandler inputHandler) : base(logger, flowStringProcessor, appState, eventBus)
    {
        InputHandler = inputHandler;
    }

    public override bool CanExecute() => true;

    public override async Task<IObservable<IVariables>> Execute(OnKeyboardShortcutTriggerData data)
    {
        return InputHandler.ListenForKeyPresses(data.KeyModifiers, data.KeysToPress)
            .Select(x => new Core.Variables.Variables());
    }
}