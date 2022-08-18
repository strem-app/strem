using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Core.Flows;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.Todos;

public class LoadTodoStorePipeline : LoadDataPipeline<TodoStore>, ILoadTodoStorePipeline
{
    public IDeserializer Deserializer { get; }
    public IEncryptor Encryptor { get; }
    
    public override string DataFilePath => $"{PathHelper.StremDataDirectory}todos.dat";
    public override bool IsEncrypted => false;

    public LoadTodoStorePipeline(PipelineBuilder pipelineBuilder, IDeserializer deserializer, IEncryptor encryptor)
        : base(pipelineBuilder, deserializer, encryptor)
    {
    }
}