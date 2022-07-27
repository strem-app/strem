using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.App;

public class LoadAppVariablesPipeline : LoadVariablesPipeline, ILoadAppVariablesPipeline
{
    public override string VariableFilePath => $"{PathHelper.AppDirectory}app.dat";
    public override bool IsEncrypted => false;
    
    public LoadAppVariablesPipeline(PipelineBuilder pipelineBuilder, IDeserializer deserializer, IEncryptor encryptor) : base(pipelineBuilder, deserializer, encryptor)
    {}
}