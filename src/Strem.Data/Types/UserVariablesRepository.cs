using LiteDB;
using Newtonsoft.Json;
using Strem.Core.Variables;
using Strem.Data.Extensions;
using JsonSerializer = LiteDB.JsonSerializer;

namespace Strem.Data.Types;

public class UserVariablesRepository : Repository<KeyValuePair<VariableEntry, string>, VariableEntry>, IUserVariablesRepository
{
    public UserVariablesRepository(ILiteDatabase connection) : base(connection, "user_variables")
    {
    }
    
    public override BsonValue GetId(VariableEntry id) => new(id.ToCompositeKey());
    
    public override BsonDocument GetDocument(KeyValuePair<VariableEntry, string> entity)
    {
        var json = JsonConvert.SerializeObject(entity);
        return JsonSerializer.Deserialize(json).AsDocument;
    }

    public override KeyValuePair<VariableEntry, string> GetEntity(BsonDocument doc)
    {
        throw new NotImplementedException();
    }
}