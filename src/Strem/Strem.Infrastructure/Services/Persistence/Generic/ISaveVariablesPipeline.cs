using Persistity.Flow.Pipelines;
using Strem.Core.State;

namespace Strem.Infrastructure.Services.Persistence.Generic;

public interface ISaveVariablesPipeline : IFlowPipeline
{
    public string VariableFilePath { get; }
    public Task Execute(IVariables variables);
}