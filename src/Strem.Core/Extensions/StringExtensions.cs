﻿using System.Text.RegularExpressions;
using Strem.Core.Types;

namespace Strem.Core.Extensions;

public static class StringExtensions
{
    public static bool MatchesText(this string? value, TextMatchType matchType, string? matchText)
    {
        if (value == null) { return matchType == TextMatchType.None; }
        if (matchText == null) { return matchType == TextMatchType.None; }
        
        return matchType switch
        {
            TextMatchType.None => true,
            TextMatchType.Contains => value.Contains(matchText),
            TextMatchType.StartsWith => value.StartsWith(matchText),
            TextMatchType.EndsWith => value.EndsWith(matchText),
            TextMatchType.ExactMatch => value == matchText,
            TextMatchType.Match => value.Equals(matchText, StringComparison.OrdinalIgnoreCase),
            _ => Regex.IsMatch(value, matchText)
        };
    }
    
    public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "…")
    {
        return value?.Length > maxLength
            ? value.Substring(0, maxLength) + truncationSuffix
            : value;
    }

    public static string ToBase64Image(this string imageFilePath)
    {
        if(!File.Exists(imageFilePath)) { return string.Empty; }
        var fileExtension = Path.GetExtension(imageFilePath);
        var base64Data = Convert.ToBase64String(File.ReadAllBytes(imageFilePath));
        return $"data:image/{fileExtension};base64, {base64Data}";
    }

    public static string GetImageUrl(this string imageFileOrUrlPath)
    {
        if(string.IsNullOrEmpty(imageFileOrUrlPath))
        { return string.Empty; }
        
        if(imageFileOrUrlPath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
           imageFileOrUrlPath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        { return imageFileOrUrlPath; }

        var actualFilePath = imageFileOrUrlPath;
        if (imageFileOrUrlPath.StartsWith("file://"))
        { actualFilePath = imageFileOrUrlPath.Replace("file:///", "").Replace("file://", ""); }

        return actualFilePath.ToBase64Image();
    }
}