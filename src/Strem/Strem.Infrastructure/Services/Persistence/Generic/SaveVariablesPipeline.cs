using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Endpoints.Files;
using Persistity.Flow.Builders;
using Persistity.Flow.Pipelines;
using Persistity.Flow.Steps.Types;
using Persistity.Processors.Encryption;
using Strem.Core.State;

namespace Strem.Infrastructure.Services.Persistence.Generic;

public abstract class SaveVariablesPipeline : FlowPipeline, ISaveVariablesPipeline
{
    public ISerializer Serializer { get; }
    public IEncryptor Encryptor { get; }

    public abstract string VariableFilePath { get; }
    public abstract bool IsEncrypted { get; }
    
    public SaveVariablesPipeline(PipelineBuilder pipelineBuilder, ISerializer serializer, IEncryptor encryptor)
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
            .ThenSendTo(new FileEndpoint(VariableFilePath))
            .BuildSteps();
    }

    public async Task Execute(IVariables variables)
    { await base.Execute(variables); }
}