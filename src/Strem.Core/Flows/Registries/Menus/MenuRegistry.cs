namespace Strem.Core.Flows.Registries.Menus;

public class MenuRegistry : IMenuRegistry
{
    public Dictionary<string, MenuDescriptor> Menus { get; } = new();
    
    public void Add(MenuDescriptor menu) => Menus.Add(menu.Code, menu);
    public void Remove(MenuDescriptor menu) => Menus.Remove(menu.Code);
    public MenuDescriptor Get(string menuCode) => Menus.ContainsKey(menuCode) ? Menus[menuCode] : null;
    public IEnumerable<MenuDescriptor> GetAll() => Menus.Values;
}