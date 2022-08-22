namespace Strem.Portals.Data;

public class ButtonStyles
{
    public string Text { get; set; }
    public string IconClass { get; set; } = "fas fa-circle-play";
    public string BackgroundColor { get; set; } = "#4a4a4a";
    public string TextColor { get; set; } = "#ffffff";

    public ButtonStyles()
    {}

    public ButtonStyles(string text, string iconClass, string backgroundColor, string textColor)
    {
        Text = text;
        IconClass = iconClass;
        BackgroundColor = backgroundColor;
        TextColor = textColor;
    }

    public ButtonStyles(ButtonStyles stylesToClone) : this(stylesToClone.Text, stylesToClone.IconClass, stylesToClone.BackgroundColor, stylesToClone.TextColor)
    { }
}