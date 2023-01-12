using LiteDB;

namespace Strem.Data;

public interface IRawQuery<out T>
{
    T Query(ILiteDatabase connection);
}