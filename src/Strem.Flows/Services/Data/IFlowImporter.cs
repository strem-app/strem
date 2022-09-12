namespace Strem.Flows.Services.Data;

public interface IFlowImporter
{
    int Import(string jsonContent);
}