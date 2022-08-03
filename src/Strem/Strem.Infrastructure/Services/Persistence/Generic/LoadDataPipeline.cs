using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Endpoints.Files;
using Persistity.Flow.Builders;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps.Types;
using Persistity.Processors.Encryption;
using Strem.Core.State;
using Strem.Core.Variables;

namespace Strem.Infrastructure.Services.Persistence.Generic;

public abstract class LoadDataPipeline<T> : FlowPipeline, ILoadDataPipeline<T>
{
    public IDeserializer Deserializer { get; }
    public IEncryptor Encryptor { get; }
    
    public abstract string DataFilePath { get; }
    public abstract bool IsEncrypted { get; }

    public LoadDataPipeline(PipelineBuilder pipelineBuilder, IDeserializer deserializer, IEncryptor encryptor)
    {
        Deserializer = deserializer;
        Encryptor = encryptor;
        Steps = BuildSteps(pipelineBuilder);
    }
    
    protected IEnumerable<IPipelineStep> BuildSteps(PipelineBuilder builder)
    {
        var buildState = builder
            .StartFrom(new FileEndpoint(DataFilePath));

        if (IsEncrypted)
        { buildState = buildState.ProcessWith(new DecryptDataProcessor(Encryptor)); }
            
        return buildState
            .DeserializeWith<T>(Deserializer)
            .BuildSteps();
    }

    public async Task<T> Execute()
    { return (T)await base.Execute(); }
}