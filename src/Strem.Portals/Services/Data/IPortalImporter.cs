namespace Strem.Portals.Services.Data;

public interface IPortalImporter
{
    int Import(string jsonContent);
}