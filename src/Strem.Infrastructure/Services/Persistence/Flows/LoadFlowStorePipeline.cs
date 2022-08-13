using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Core.Flows;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.Flows;

public class LoadFlowStorePipeline : LoadDataPipeline<FlowStore>, ILoadFlowStorePipeline
{
    public IDeserializer Deserializer { get; }
    public IEncryptor Encryptor { get; }
    
    public override string DataFilePath => $"{PathHelper.AppDirectory}flows.dat";
    public override bool IsEncrypted => false;

    public LoadFlowStorePipeline(PipelineBuilder pipelineBuilder, IDeserializer deserializer, IEncryptor encryptor)
        : base(pipelineBuilder, deserializer, encryptor)
    {
    }
}