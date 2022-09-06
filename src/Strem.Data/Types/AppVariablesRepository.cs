using LiteDB;
using Strem.Core.Variables;
using Strem.Data.Extensions;

namespace Strem.Data.Types;

public class AppVariablesRepository : Repository<KeyValuePair<VariableEntry, string>, VariableEntry>, IAppVariablesRepository
{
    public AppVariablesRepository(ILiteDatabase connection) : base(connection, "app-variables")
    {
    }

    public override BsonValue GetId(VariableEntry id) => new(id.ToCompositeKey());
}