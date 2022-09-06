using Strem.Core.Variables;

namespace Strem.Data.Types;

public interface IAppVariablesRepository : IRepository<KeyValuePair<VariableEntry, string>, VariableEntry>
{}