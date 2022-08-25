using Strem.Portals.Data;
using Strem.Portals.Data.Overrides;

namespace Strem.Portals.Extensions;

public static class ButtonRuntimeStylesExtensions
{
    public static ButtonStyles GetButtonStyles(this ButtonRuntimeStyles styles, Guid portalId, Guid buttonId)
    {
        if(!styles.RuntimeStyles.ContainsKey(portalId)) { return null; }
        if(!styles.RuntimeStyles[portalId].ContainsKey(buttonId)) { return null; }
        return styles.RuntimeStyles[portalId][buttonId];
    }
    
    public static string GetButtonText(this ButtonRuntimeStyles styles, Guid portalId, Guid buttonId)
    {
        var buttonStyles = styles.GetButtonStyles(portalId, buttonId);
        return buttonStyles?.Text ?? string.Empty;
    }
    
    public static string GetButtonIcon(this ButtonRuntimeStyles styles, Guid portalId, Guid buttonId)
    {
        var buttonStyles = styles.GetButtonStyles(portalId, buttonId);
        return buttonStyles?.IconClass ?? string.Empty;
    }
    
    public static string GetButtonBackgroundColor(this ButtonRuntimeStyles styles, Guid portalId, Guid buttonId)
    {
        var buttonStyles = styles.GetButtonStyles(portalId, buttonId);
        return buttonStyles?.BackgroundColor ?? string.Empty;
    }    
    
    public static string GetButtonTextColor(this ButtonRuntimeStyles styles, Guid portalId, Guid buttonId)
    {
        var buttonStyles = styles.GetButtonStyles(portalId, buttonId);
        return buttonStyles?.Text ?? string.Empty;
    }
}