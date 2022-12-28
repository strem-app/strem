using LiteDB;
using Newtonsoft.Json;
using JsonSerializer = LiteDB.JsonSerializer;

namespace Strem.Data;

public abstract class Repository<TE,TK> : IRepository<TE, TK>
{
    public ILiteDatabase Connection { get; }
    public ILiteCollection<BsonDocument> Collection { get; }

    public Repository(ILiteDatabase connection, string collectionName)
    {
        Connection = connection;
        Collection = Connection.GetCollection(collectionName);
    }

    public abstract BsonValue GetId(TK id);

    
    public virtual BsonDocument GetDocument(TE entity)
    {
        var json = JsonConvert.SerializeObject(entity);
        return JsonSerializer.Deserialize(json).AsDocument;
    }

    public virtual TE? GetEntity(BsonDocument doc)
    {
        var json = doc.ToString();
        return JsonConvert.DeserializeObject<TE>(json);
    }

    public TE? Get(TK id)
    {
        var document = Collection.FindById(GetId(id));
        return document == null ? default : GetEntity(document);
    }

    public IEnumerable<TE> GetAll()
    { return Collection.FindAll().Select(GetEntity); }

    public IEnumerable<TE> Query(Query query)
    { return Collection.Find(query).Select(GetEntity); }

    public void Create(TK id, TE entity)
    {
        var document = GetDocument(entity);
        Collection.Insert(GetId(id), document);
    }

    public bool Update(TK id, TE entity)
    {
        var document = GetDocument(entity);
        return Collection.Update(GetId(id), document);
    }
    
    public bool Upsert(TK id, TE entity)
    {
        var document = GetDocument(entity);
        return Collection.Upsert(GetId(id), document);
    }

    public void Delete(TK id) => Collection.Delete(GetId(id));
    
    public IEnumerable<TE?> Query(IBsonQuery query)
    {
        var queryCollection = Collection.Query();
        return query.Query(queryCollection)
            .ToEnumerable()
            .Select(GetEntity);
    }
}