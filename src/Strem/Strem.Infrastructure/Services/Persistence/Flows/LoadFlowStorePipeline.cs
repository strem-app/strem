using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Endpoints.Files;
using Persistity.Flow.Builders;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps.Types;
using Persistity.Processors.Encryption;
using Strem.Core.Flows;

namespace Strem.Infrastructure.Services.Persistence.Flows;

public class LoadFlowStorePipeline : FlowPipeline, ILoadFlowStorePipeline
{
    public IDeserializer Deserializer { get; }
    public IEncryptor Encryptor { get; }
    
    public string FlowStoreFilePath => $"{PathHelper.AppDirectory}flows.dat";
    public bool IsEncrypted { get; set; } = false;

    public LoadFlowStorePipeline(PipelineBuilder pipelineBuilder, IDeserializer deserializer, IEncryptor encryptor)
    {
        Deserializer = deserializer;
        Encryptor = encryptor;
        Steps = BuildSteps(pipelineBuilder);
    }
    
    protected IEnumerable<IPipelineStep> BuildSteps(PipelineBuilder builder)
    {
        var buildState = builder
            .StartFrom(new FileEndpoint(FlowStoreFilePath));

        if (IsEncrypted)
        { buildState = buildState.ProcessWith(new DecryptDataProcessor(Encryptor)); }
            
        return buildState
            .DeserializeWith<FlowStore>(Deserializer)
            .BuildSteps();
    }

    public async Task<FlowStore> Execute()
    { return (FlowStore)await base.Execute(); }
}