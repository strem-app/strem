using Persistity.Flow.Pipelines;
using Strem.Core.Flows;

namespace Strem.Infrastructure.Services.Persistence.Flows;

public interface ISaveFlowStorePipeline : IFlowPipeline
{
    public Task Execute(FlowStore flowStore);
    string FlowStoreFilePath { get; }
}