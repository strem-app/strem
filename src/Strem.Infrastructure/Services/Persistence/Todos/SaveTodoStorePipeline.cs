using Persistity.Core.Serialization;
using Persistity.Encryption;
using Persistity.Flow.Builders;
using Strem.Core.Flows;
using Strem.Infrastructure.Services.Persistence.Generic;

namespace Strem.Infrastructure.Services.Persistence.Todos;

public class SaveTodoStorePipeline : SaveDataPipeline<ITodoStore>, ISaveTodoStorePipeline
{
    public override string DataFilePath => $"{PathHelper.StremDataDirectory}todos.dat";
    public override bool IsEncrypted => false;

    public SaveTodoStorePipeline(PipelineBuilder pipelineBuilder, ISerializer serializer, IEncryptor encryptor) : base(pipelineBuilder, serializer, encryptor)
    {
    }
}