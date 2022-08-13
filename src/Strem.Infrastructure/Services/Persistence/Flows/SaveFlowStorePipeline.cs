using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Core.Flows;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.Flows;

public class SaveFlowStorePipeline : SaveDataPipeline<IFlowStore>, ISaveFlowStorePipeline
{
    public override string DataFilePath => $"{PathHelper.AppDirectory}flows.dat";
    public override bool IsEncrypted => false;

    public SaveFlowStorePipeline(PipelineBuilder pipelineBuilder, ISerializer serializer, IEncryptor encryptor) : base(pipelineBuilder, serializer, encryptor)
    {
    }
}