namespace Strem.Core.Flows;

public interface IFlowElementData
{
    Guid Id { get; set; }
    string Version { get; set; }
    string Code { get; }
}