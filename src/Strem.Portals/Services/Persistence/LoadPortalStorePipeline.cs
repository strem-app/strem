using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Infrastructure.Services.Persistence;
using Strem.Infrastructure.Services.Persistence.Generic;
using Strem.Portals.Data;

namespace Strem.Portals.Services.Persistence;

public class LoadPortalStorePipeline : LoadDataPipeline<PortalStore>, ILoadPortalStorePipeline
{
    public IDeserializer Deserializer { get; }
    public IEncryptor Encryptor { get; }
    
    public override string DataFilePath => $"{PathHelper.StremDataDirectory}portals.dat";
    public override bool IsEncrypted => false;

    public LoadPortalStorePipeline(PipelineBuilder pipelineBuilder, IDeserializer deserializer, IEncryptor encryptor)
        : base(pipelineBuilder, deserializer, encryptor)
    {
    }
}