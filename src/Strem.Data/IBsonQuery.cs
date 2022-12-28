using LiteDB;

namespace Strem.Data;

public interface IBsonQuery
{
    ILiteQueryable<BsonDocument> Query(ILiteQueryable<BsonDocument> queryableDocuments);
}