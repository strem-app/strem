using LiteDB;

namespace Strem.Data;

public interface IRepository<TE, in TK>
{
    public TE? Get(TK id);
    public IEnumerable<TE> GetAll();
    void Create(TK id, TE entity);
    bool Update(TK id, TE entity);
    void Delete(TK id);
    bool Upsert(TK id, TE entity);
    IEnumerable<TE?> Query(IBsonQuery query);
    T Query<T>(IRawQuery<T> query);
}