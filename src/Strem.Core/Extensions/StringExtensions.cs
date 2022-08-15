using System.Text.RegularExpressions;
using Strem.Core.Types;

namespace Strem.Core.Extensions;

public static class StringExtensions
{
    public static bool MatchesText(this string value, TextMatchType matchTypeType, string matchText)
    {
        return matchTypeType switch
        {
            TextMatchType.None => true,
            TextMatchType.Contains => value.Contains(matchText),
            TextMatchType.StartsWith => value.StartsWith(matchText),
            TextMatchType.EndsWith => value.EndsWith(matchText),
            TextMatchType.ExactMatch => value == matchText,
            _ => Regex.IsMatch(value, matchText)
        };
    }
}