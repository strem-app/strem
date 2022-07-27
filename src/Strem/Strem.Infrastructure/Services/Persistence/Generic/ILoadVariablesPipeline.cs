using Persistity.Flow.Pipelines;
using Strem.Core.State;

namespace Strem.Infrastructure.Services.Persistence.Generic;

public interface ILoadVariablesPipeline : IFlowPipeline
{
    public string VariableFilePath { get; }
    public Task<IVariables> Execute();
}