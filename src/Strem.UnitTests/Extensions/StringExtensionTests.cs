using Strem.Core.Extensions;
using Strem.Core.Types;

namespace Strem.UnitTests.Extensions;

public class StringExtensionTests
{
    [Theory]
    [InlineData("", "", TextMatchType.None, true)]
    [InlineData("a", "b", TextMatchType.None, true)]
    [InlineData("", "", TextMatchType.None, true)]
    [InlineData("", null, TextMatchType.None, true)]
    [InlineData(null, null, TextMatchType.None, true)]
    [InlineData("a", "b", TextMatchType.ExactMatch, false)]
    [InlineData("hello you", "hello me", TextMatchType.ExactMatch, false)]
    [InlineData("hello you", "hello you", TextMatchType.ExactMatch, true)]
    [InlineData("a", "a", TextMatchType.ExactMatch, true)]
    [InlineData("1", "1", TextMatchType.ExactMatch, true)]
    [InlineData(null, "1", TextMatchType.ExactMatch, false)]
    [InlineData(null, "", TextMatchType.ExactMatch, false)]
    [InlineData(null, null, TextMatchType.ExactMatch, false)]
    [InlineData("", null, TextMatchType.ExactMatch, false)]
    [InlineData("", "", TextMatchType.ExactMatch, true)]
    [InlineData("1", "one", TextMatchType.ExactMatch, false)]
    [InlineData("a", "b", TextMatchType.Contains, false)]
    [InlineData("bab", "a", TextMatchType.Contains, true)]
    [InlineData("bab", "c", TextMatchType.Contains, false)]
    [InlineData("a", "bab", TextMatchType.Contains, false)]
    [InlineData("bab", "", TextMatchType.Contains, true)]
    [InlineData("", "", TextMatchType.Contains, true)]
    [InlineData(null, "", TextMatchType.Contains, false)]
    [InlineData("", null, TextMatchType.Contains, false)]
    [InlineData("hello there", "hello", TextMatchType.StartsWith, true)]
    [InlineData("hello there", "hello there", TextMatchType.StartsWith, true)]
    [InlineData("hello there", "hello there man", TextMatchType.StartsWith, false)]
    [InlineData("a", "b", TextMatchType.StartsWith, false)]
    [InlineData("", "", TextMatchType.StartsWith, true)]
    [InlineData("a", "b", TextMatchType.EndsWith, false)]
    [InlineData("hello", "b", TextMatchType.EndsWith, false)]
    [InlineData("hello", "o", TextMatchType.EndsWith, true)]
    [InlineData("hello", "llo", TextMatchType.EndsWith, true)]
    [InlineData("hello", "hello", TextMatchType.EndsWith, true)]
    [InlineData("hello", " hello", TextMatchType.EndsWith, false)]
    [InlineData("", "", TextMatchType.EndsWith, true)]
    [InlineData("a", "a", TextMatchType.EndsWith, true)]
    [InlineData("a", @".*", TextMatchType.Pattern, true)]
    [InlineData("a", @"[0-9]{1}", TextMatchType.Pattern, false)]
    [InlineData("1", @"[0-9]{1}", TextMatchType.Pattern, true)]
    [InlineData("5", @"[0-9]{1}", TextMatchType.Pattern, true)]
    [InlineData("", @"[0-9]{1}", TextMatchType.Pattern, false)]
    [InlineData("", @"", TextMatchType.Pattern, true)]
    [InlineData(null, @"[0-9]{1}", TextMatchType.Pattern, false)]
    public void should_match_correctly(string valueA, string valueB, TextMatchType matchType, bool expectedMatch)
    {
        var actualMatch = valueA.MatchesText(matchType, valueB);
        Assert.Equal(expectedMatch, actualMatch);
    }
}