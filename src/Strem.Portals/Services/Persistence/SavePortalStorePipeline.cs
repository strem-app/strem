using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Infrastructure.Services.Persistence;
using Strem.Infrastructure.Services.Persistence.Generic;
using Strem.Portals.Data;

namespace Strem.Portals.Services.Persistence;

public class SavePortalStorePipeline : SaveDataPipeline<IPortalStore>, ISavePortalStorePipeline
{
    public override string DataFilePath => $"{PathHelper.StremDataDirectory}portals.dat";
    public override bool IsEncrypted => false;

    public SavePortalStorePipeline(PipelineBuilder pipelineBuilder, ISerializer serializer, IEncryptor encryptor) : base(pipelineBuilder, serializer, encryptor)
    {
    }
}