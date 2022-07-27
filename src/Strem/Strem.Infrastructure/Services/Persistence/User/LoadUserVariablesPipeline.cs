using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.User;

public class LoadUserVariablesPipeline : LoadVariablesPipeline, ILoadUserVariablesPipeline
{
    public override string VariableFilePath => $"{PathHelper.AppDirectory}user.dat";
    public override bool IsEncrypted => false;

    public LoadUserVariablesPipeline(PipelineBuilder pipelineBuilder, IDeserializer deserializer, IEncryptor encryptor)
        : base(pipelineBuilder, deserializer, encryptor)
    {
    }
}