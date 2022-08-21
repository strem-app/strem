using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Core.Portals;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.Portals;

public class SavePortalStorePipeline : SaveDataPipeline<IPortalStore>, ISavePortalStorePipeline
{
    public override string DataFilePath => $"{PathHelper.StremDataDirectory}portals.dat";
    public override bool IsEncrypted => false;

    public SavePortalStorePipeline(PipelineBuilder pipelineBuilder, ISerializer serializer, IEncryptor encryptor) : base(pipelineBuilder, serializer, encryptor)
    {
    }
}