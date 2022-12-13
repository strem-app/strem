using Strem.Portals.Data;
using Strem.Portals.Types;

namespace Strem.Portals.Extensions;

public static class ElementStylesExtensions
{
    public static string ImageUrlKey = "image-url";
    public static string IconClassKey = "icon-class";
    public static string ButtonTypeKey = "button-type";
    public static string SliderCellSizeKey = "slider-cell-size";

    public static string GetFromKey(ElementStyles elementStyles, string key)
    {
        return elementStyles.CustomStyles.ContainsKey(key)
            ? elementStyles.CustomStyles[key]
            : string.Empty;
    }
    
    public static void SetFromKey(ElementStyles elementStyles, string key, string value)
    { elementStyles.CustomStyles[key] = value; }

    public static string ImageUrl(this ElementStyles elementStyles) => GetFromKey(elementStyles, ImageUrlKey);
    public static void ImageUrl(this ElementStyles elementStyles, string url) => SetFromKey(elementStyles, ImageUrlKey, url);
    public static string IconClass(this ElementStyles elementStyles) => GetFromKey(elementStyles, IconClassKey);
    public static void IconClass(this ElementStyles elementStyles, string iconClasses) => SetFromKey(elementStyles, IconClassKey, iconClasses);
    
    public static int CellSize(this ElementStyles elementStyles)
    {
        var value = GetFromKey(elementStyles, SliderCellSizeKey);
        if (string.IsNullOrEmpty(value)) { return 2; }
        return int.TryParse(value, out var parsedValue) ? parsedValue : 2;
    }

    public static void CellSize(this ElementStyles elementStyles, int cellSize)
    { SetFromKey(elementStyles, SliderCellSizeKey, cellSize.ToString()); }

    public static ButtonType ButtonType(this ElementStyles elementStyles)
    {
        var value = GetFromKey(elementStyles, ButtonTypeKey);
        if (string.IsNullOrEmpty(value)) { return Types.ButtonType.IconButton; }
        return int.TryParse(value, out var parsedValue) ? (ButtonType)parsedValue : Types.ButtonType.IconButton;
    }
    
    public static void ButtonType(this ElementStyles elementStyles, ButtonType buttonType)
    { SetFromKey(elementStyles, SliderCellSizeKey, ((int)buttonType).ToString()); }
}