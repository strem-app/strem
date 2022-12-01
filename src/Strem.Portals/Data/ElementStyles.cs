namespace Strem.Portals.Data;

public class ElementStyles
{
    public string Text { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = "#4a4a4a";

    public ElementStyles()
    {
    }

    public ElementStyles(string text, string backgroundColor)
    {
        BackgroundColor = backgroundColor;
        Text = text;
    }
}