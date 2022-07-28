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

public abstract class LoadVariablesPipeline : FlowPipeline, ILoadVariablesPipeline
{
    public IDeserializer Deserializer { get; }
    public IEncryptor Encryptor { get; }
    
    public abstract string VariableFilePath { get; }
    public abstract bool IsEncrypted { get; }

    public LoadVariablesPipeline(PipelineBuilder pipelineBuilder, IDeserializer deserializer, IEncryptor encryptor)
    {
        Deserializer = deserializer;
        Encryptor = encryptor;
        Steps = BuildSteps(pipelineBuilder);
    }
    
    protected IEnumerable<IPipelineStep> BuildSteps(PipelineBuilder builder)
    {
        var buildState = builder
            .StartFrom(new FileEndpoint(VariableFilePath));

        if (IsEncrypted)
        { buildState = buildState.ProcessWith(new DecryptDataProcessor(Encryptor)); }
            
        return buildState
            .DeserializeWith<Variables>(Deserializer)
            .BuildSteps();
    }

    public async Task<IVariables> Execute()
    { return (Variables)await base.Execute(); }
}