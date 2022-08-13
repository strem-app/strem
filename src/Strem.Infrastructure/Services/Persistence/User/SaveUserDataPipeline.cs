using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Core.Variables;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.User;

public class SaveUserDataPipeline : SaveDataPipeline<IVariables>, ISaveUserDataPipeline
{
    public override string DataFilePath => $"{PathHelper.AppDirectory}user.dat";
    public override bool IsEncrypted => false;
    
    public SaveUserDataPipeline(PipelineBuilder pipelineBuilder, ISerializer serializer, IEncryptor encryptor) : base(pipelineBuilder, serializer, encryptor)
    {
    }
}