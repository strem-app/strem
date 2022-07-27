using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.User;

public class SaveUserVariablesPipeline : SaveVariablesPipeline, ISaveUserVariablesPipeline
{
    public override string VariableFilePath => $"{PathHelper.AppDirectory}user.dat";
    public override bool IsEncrypted => false;
    
    public SaveUserVariablesPipeline(PipelineBuilder pipelineBuilder, ISerializer serializer, IEncryptor encryptor) : base(pipelineBuilder, serializer, encryptor)
    {
    }
}