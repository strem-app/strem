namespace Strem.Portals.Data;

public class ElementStyles
{
    public string Text { get; set; } = string.Empty;
    public string BackgroundColor { get; set; } = "#4a4a4a";
    public string ForegroundColor { get; set; } = "#ffffff";
    public Dictionary<string, string> CustomStyles { get; set; } = new();

    public ElementStyles()
    {}

    public ElementStyles(string text, string backgroundColor, string foregroundColor, IReadOnlyDictionary<string, string> customStyles)
    {
        Text = text;
        BackgroundColor = backgroundColor;
        ForegroundColor = foregroundColor;
        CustomStyles = customStyles.ToDictionary(x => x.Key, x => x.Value);
    }

    public ElementStyles(ElementStyles stylesToClone) : 
        this(stylesToClone.Text, stylesToClone.BackgroundColor, stylesToClone.ForegroundColor, stylesToClone.CustomStyles)
    { }
}