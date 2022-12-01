using Strem.Portals.Types;

namespace Strem.Portals.Data;

public class ButtonStyles : ElementStyles
{
    public string IconClass { get; set; } = "fas fa-circle-play";
    public string ImageUrl { get; set; } = string.Empty;
    public string TextColor { get; set; } = "#ffffff";
    public ButtonType ButtonType { get; set; } = ButtonType.IconButton;

    public ButtonStyles()
    {}

    public ButtonStyles(string text, string iconClass, string imageUrl, string backgroundColor, string textColor, ButtonType type) : base(text, backgroundColor)
    {
        IconClass = iconClass;
        ImageUrl = imageUrl;
        TextColor = textColor;
        ButtonType = type;
    }

    public ButtonStyles(ButtonStyles stylesToClone) : 
        this(stylesToClone.Text, stylesToClone.IconClass, stylesToClone.ImageUrl, stylesToClone.BackgroundColor, stylesToClone.TextColor, stylesToClone.ButtonType)
    { }
}