using Persistity.Flow.Pipelines;

namespace Strem.Infrastructure.Services.Persistence.Generic;

public interface ILoadDataPipeline<T> : IFlowPipeline
{
    public string DataFilePath { get; }
    public Task<T> Execute();
}