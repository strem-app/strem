
namespace Strem.Portals.Data;

public class ButtonData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Text { get; set; }
    public string IconClass { get; set; } = "fas fa-circle-play";
    public string BackgroundColor { get; set; } = "#4a4a4a";
    public string TextColor { get; set; } = "#ffffff";
}