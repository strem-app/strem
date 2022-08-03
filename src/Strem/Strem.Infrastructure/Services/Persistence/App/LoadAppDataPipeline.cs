using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Core.Variables;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.App;

public class LoadAppDataPipeline : LoadDataPipeline<Variables>, ILoadAppDataPipeline
{
    public override string DataFilePath => $"{PathHelper.AppDirectory}app.dat";
    public override bool IsEncrypted => false;
    
    public LoadAppDataPipeline(PipelineBuilder pipelineBuilder, IDeserializer deserializer, IEncryptor encryptor) 
        : base(pipelineBuilder, deserializer, encryptor)
    {}
}