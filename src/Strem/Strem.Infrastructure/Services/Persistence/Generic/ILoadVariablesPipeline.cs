using Persistity.Flow.Pipelines;
using Strem.Core.Variables;

namespace Strem.Infrastructure.Services.Persistence.Generic;

public interface ILoadVariablesPipeline : IFlowPipeline
{
    public string VariableFilePath { get; }
    public Task<IVariables> Execute();
}