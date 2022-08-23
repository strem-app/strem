using Strem.Portals.Types;

namespace Strem.Portals.Data;

public class ButtonStyles
{
    public string Text { get; set; }
    public string IconClass { get; set; } = "fas fa-circle-play";
    public string ImageUrl { get; set; }
    public string BackgroundColor { get; set; } = "#4a4a4a";
    public string TextColor { get; set; } = "#ffffff";
    public ButtonType ButtonType { get; set; } = ButtonType.IconButton;

    public ButtonStyles()
    {}

    public ButtonStyles(string text, string iconClass, string imageUrl, string backgroundColor, string textColor, ButtonType type)
    {
        Text = text;
        IconClass = iconClass;
        ImageUrl = imageUrl;
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        ButtonType = type;
    }

    public ButtonStyles(ButtonStyles stylesToClone) : 
        this(stylesToClone.Text, stylesToClone.IconClass, stylesToClone.ImageUrl, stylesToClone.BackgroundColor, stylesToClone.TextColor, stylesToClone.ButtonType)
    { }
}