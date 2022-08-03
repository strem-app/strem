using Persistity.Flow.Pipelines;

namespace Strem.Infrastructure.Services.Persistence.Generic;

public interface ISaveDataPipeline<T> : IFlowPipeline
{
    public string DataFilePath { get; }
    public Task Execute(T variables);
}