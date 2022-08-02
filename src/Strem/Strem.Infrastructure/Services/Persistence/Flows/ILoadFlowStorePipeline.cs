using Persistity.Flow.Pipelines;
using Strem.Core.Flows;

namespace Strem.Infrastructure.Services.Persistence.Flows;

public interface ILoadFlowStorePipeline : IFlowPipeline
{
    public Task<FlowStore> Execute();
    string FlowStoreFilePath { get; }
}