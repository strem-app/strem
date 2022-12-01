using Strem.Portals.Types;

namespace Strem.Portals.Data;

public class ElementStyles
{
    public ButtonType ButtonType { get; set; } = ButtonType.IconButton;
    public string Text { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = "#4a4a4a";
    public string ForegroundColor { get; set; } = "#ffffff";
    public Dictionary<string, string> CustomStyles { get; set; } = new();

    public ElementStyles()
    {}

    public ElementStyles(ButtonType type, string text, string backgroundColor, string foregroundColor, IReadOnlyDictionary<string, string> customStyles)
    {
        Text = text;
        BackgroundColor = backgroundColor;
        ForegroundColor = foregroundColor;
        ButtonType = type;
        CustomStyles = customStyles.ToDictionary(x => x.Key, x => x.Value);
    }

    public ElementStyles(ElementStyles stylesToClone) : 
        this(stylesToClone.ButtonType, stylesToClone.Text, stylesToClone.BackgroundColor, stylesToClone.ForegroundColor, stylesToClone.CustomStyles)
    { }
}