﻿using Strem.Portals.Data;
using Strem.Portals.Data.Overrides;
using Strem.Portals.Services.Stores;

namespace Strem.Portals.Extensions;

public static class ButtonRuntimeStylesExtensions
{
    public static GridElementRuntimeStyles PopulateRuntimeStyles(this GridElementRuntimeStyles styles, IPortalStore portalStore)
    {
        var buttonRuntimeStyles = new GridElementRuntimeStyles();
        foreach (var portal in portalStore.Data)
        {
            var buttonStyles = portal.Elements
                .ToDictionary(x => x.Id, x => new ElementStyles(x.DefaultStyles));
            buttonRuntimeStyles.RuntimeStyles.Add(portal.Id, buttonStyles);
        }
        return buttonRuntimeStyles;
    }
    
    public static void RefreshStylesFor(this GridElementRuntimeStyles styles, Guid portalId, GridElementData gridElement)
    {
        if(!styles.RuntimeStyles.ContainsKey(portalId))
        { return; }

        var portalStyles = styles.RuntimeStyles[portalId];
        if(!portalStyles.ContainsKey(gridElement.Id))
        { return; }

        portalStyles[gridElement.Id] = new ElementStyles(gridElement.DefaultStyles);
    }
    
    public static ElementStyles GetButtonStyles(this GridElementRuntimeStyles styles, Guid portalId, Guid buttonId)
    {
        if(!styles.RuntimeStyles.ContainsKey(portalId)) { return null; }
        if(!styles.RuntimeStyles[portalId].ContainsKey(buttonId)) { return null; }
        return styles.RuntimeStyles[portalId][buttonId];
    }
    
    public static string GetButtonText(this GridElementRuntimeStyles styles, Guid portalId, Guid buttonId)
    {
        var buttonStyles = styles.GetButtonStyles(portalId, buttonId);
        return buttonStyles?.Text ?? string.Empty;
    }
    
    public static string GetButtonIcon(this GridElementRuntimeStyles styles, Guid portalId, Guid buttonId)
    {
        var buttonStyles = styles.GetButtonStyles(portalId, buttonId);
        return buttonStyles?.IconClass() ?? string.Empty;
    }
    
    public static string GetButtonBackgroundColor(this GridElementRuntimeStyles styles, Guid portalId, Guid buttonId)
    {
        var buttonStyles = styles.GetButtonStyles(portalId, buttonId);
        return buttonStyles?.BackgroundColor ?? string.Empty;
    }    
    
    public static string GetButtonTextColor(this GridElementRuntimeStyles styles, Guid portalId, Guid buttonId)
    {
        var buttonStyles = styles.GetButtonStyles(portalId, buttonId);
        return buttonStyles?.Text ?? string.Empty;
    }
}