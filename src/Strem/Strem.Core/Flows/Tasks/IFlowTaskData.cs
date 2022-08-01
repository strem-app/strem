namespace Strem.Core.Flows.Tasks;

public interface IFlowTaskData
{
    string Version { get; }
    string TaskCode { get; }
}