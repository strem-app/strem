using Strem.Portals.Data;

namespace Strem.Portals.Extensions;

public static class ElementStylesExtensions
{
    public static string ImageUrlKey = "image-url";
    public static string IconClassKey = "icon-class";

    public static string GetFromKey(ElementStyles elementStyles, string key)
    {
        return elementStyles.CustomStyles.ContainsKey(ImageUrlKey)
            ? elementStyles.CustomStyles[ImageUrlKey]
            : string.Empty;
    }
    
    public static string SetFromKey(ElementStyles elementStyles, string key, string value)
    { return elementStyles.CustomStyles[ImageUrlKey] = value; }

    public static string ImageUrl(this ElementStyles elementStyles) => GetFromKey(elementStyles, ImageUrlKey);
    public static void ImageUrl(this ElementStyles elementStyles, string url) => SetFromKey(elementStyles, ImageUrlKey, url);
    public static string IconClass(this ElementStyles elementStyles) => GetFromKey(elementStyles, IconClassKey);
    public static void IconClass(this ElementStyles elementStyles, string iconClasses) => SetFromKey(elementStyles, IconClassKey, iconClasses);

}