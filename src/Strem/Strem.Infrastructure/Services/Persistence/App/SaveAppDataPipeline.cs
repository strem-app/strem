using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Core.Variables;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.App;

public class SaveAppDataPipeline : SaveDataPipeline<IVariables>, ISaveAppDataPipeline
{
    public override string DataFilePath => $"{PathHelper.AppDirectory}app.dat";
    public override bool IsEncrypted => false;
    
    public SaveAppDataPipeline(PipelineBuilder pipelineBuilder, ISerializer serializer, IEncryptor encryptor) : base(pipelineBuilder, serializer, encryptor)
    {
    }
}