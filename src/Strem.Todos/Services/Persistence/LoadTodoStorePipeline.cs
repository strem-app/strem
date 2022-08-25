using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Infrastructure.Services.Persistence;
using Strem.Infrastructure.Services.Persistence.Generic;
using Strem.Todos.Data;

namespace Strem.Todos.Services.Persistence;

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