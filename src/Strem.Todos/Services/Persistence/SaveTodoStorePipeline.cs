using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Infrastructure.Services.Persistence;
using Strem.Infrastructure.Services.Persistence.Generic;
using Strem.Todos.Data;

namespace Strem.Todos.Services.Persistence;

public class SaveTodoStorePipeline : SaveDataPipeline<ITodoStore>, ISaveTodoStorePipeline
{
    public override string DataFilePath => $"{PathHelper.StremDataDirectory}todos.dat";
    public override bool IsEncrypted => false;

    public SaveTodoStorePipeline(PipelineBuilder pipelineBuilder, ISerializer serializer, IEncryptor encryptor) : base(pipelineBuilder, serializer, encryptor)
    {
    }
}