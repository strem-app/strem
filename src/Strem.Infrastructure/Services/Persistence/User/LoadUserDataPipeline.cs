using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Core.Variables;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.User;

public class LoadUserDataPipeline : LoadDataPipeline<Variables>, ILoadUserDataPipeline
{
    public override string DataFilePath => $"{PathHelper.StremDataDirectory}user.dat";
    public override bool IsEncrypted => false;

    public LoadUserDataPipeline(PipelineBuilder pipelineBuilder, IDeserializer deserializer, IEncryptor encryptor)
        : base(pipelineBuilder, deserializer, encryptor)
    {
    }
}