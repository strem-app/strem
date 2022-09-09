namespace Strem.Flows.Data;

public interface IFlowElementData
{
    Guid Id { get; set; }
    string Version { get; set; }
    string Code { get; }
}