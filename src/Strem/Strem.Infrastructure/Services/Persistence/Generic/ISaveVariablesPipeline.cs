using Persistity.Flow.Pipelines;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Infrastructure.Services.Persistence.Generic;

public interface ISaveVariablesPipeline : IFlowPipeline
{
    public string VariableFilePath { get; }
    public Task Execute(IVariables variables);
}