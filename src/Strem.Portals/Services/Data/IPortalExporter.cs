namespace Strem.Portals.Services.Data;

public interface IPortalExporter
{
    string Export(IEnumerable<Guid> ids);
}