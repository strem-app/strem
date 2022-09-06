using Strem.Core.Variables;

namespace Strem.Data.Types;

public interface IUserVariablesRepository : IRepository<KeyValuePair<VariableEntry, string>, VariableEntry>
{}