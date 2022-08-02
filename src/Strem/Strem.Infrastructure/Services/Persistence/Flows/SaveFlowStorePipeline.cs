using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Endpoints.Files;
using Persistity.Flow.Builders;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps.Types;
using Persistity.Processors.Encryption;
using Strem.Core.Flows;

namespace Strem.Infrastructure.Services.Persistence.Flows;

public class SaveFlowStorePipeline : FlowPipeline, ISaveFlowStorePipeline
{
    public ISerializer Serializer { get; }
    public IEncryptor Encryptor { get; }

    public string FlowStoreFilePath => $"{PathHelper.AppDirectory}flows.dat";
    public bool IsEncrypted { get; set; } = false;
    
    public SaveFlowStorePipeline(PipelineBuilder pipelineBuilder, ISerializer serializer, IEncryptor encryptor)
    {
        Serializer = serializer;
        Encryptor = encryptor;
        Steps = BuildSteps(pipelineBuilder);
    }
    
    protected IEnumerable<IPipelineStep> BuildSteps(PipelineBuilder builder)
    {
        var buildState = builder
            .StartFromInput()
            .SerializeWith(Serializer);

        if (IsEncrypted)
        { buildState = buildState.ProcessWith(new EncryptDataProcessor(Encryptor)); }
            
        return buildState
            .ThenSendTo(new FileEndpoint(FlowStoreFilePath))
            .BuildSteps();
    }

    public async Task Execute(FlowStore variables)
    { await base.Execute(variables); }
}