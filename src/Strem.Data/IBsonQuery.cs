using LiteDB;

namespace Strem.Data;

public interface IBsonQuery
{
    IEnumerable<BsonDocument> Query(ILiteQueryable<BsonDocument> queryableDocuments);
}