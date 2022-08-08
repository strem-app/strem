using Strem.Core.Flows.Tasks;
using WindowsInput.Native;

namespace Strem.Flows.Default.Flows.Tasks.Data;

public class SimulateKeyPressTaskData : IFlowTaskData
{
    public static readonly string TaskCode = "simulate-key-press";
    public static readonly string TaskVersion = "1.0.0";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code => TaskCode;
    public string Version { get; set; } = TaskVersion;
    
    public List<VirtualKeyCode> KeyModifiers { get; set; }
    public List<VirtualKeyCode> KeysToPress { get; set; }
}