using Strem.Portals.Data;
using Strem.Portals.Data.Overrides;
using Strem.Portals.Services.Stores;

namespace Strem.Portals.Extensions;

public static class ButtonRuntimeStylesExtensions
{
    public static ButtonRuntimeStyles PopulateRuntimeStyles(this ButtonRuntimeStyles styles, IPortalStore portalStore)
    {
        var buttonRuntimeStyles = new ButtonRuntimeStyles();
        foreach (var portal in portalStore.Data)
        {
            var buttonStyles = portal.Buttons
                .ToDictionary(x => x.Id, x => new ButtonStyles(x.DefaultStyles));
            buttonRuntimeStyles.RuntimeStyles.Add(portal.Id, buttonStyles);
        }
        return buttonRuntimeStyles;
    }
    
    public static void RefreshStylesFor(this ButtonRuntimeStyles styles, Guid portalId, ButtonData button)
    {
        if(!styles.RuntimeStyles.ContainsKey(portalId))
        { return; }

        var portalStyles = styles.RuntimeStyles[portalId];
        if(!portalStyles.ContainsKey(button.Id))
        { return; }

        portalStyles[button.Id] = new ButtonStyles(button.DefaultStyles);
    }
    
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