namespace Strem.Core.Services.Registries.Menus;

public class MenuRegistry : Registry<MenuDescriptor>, IMenuRegistry
{
    public override string GetId(MenuDescriptor data) => data.Code;
}