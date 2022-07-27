using Strem.Core.Utils;

namespace Strem.Core.Extensions;

public static class IRandomizerExtensions
{
    public static int[] AsciiCharCodes = Enumerable.Range('A', 'Z' - 'A' + 1).ToArray();
    
    public static string RandomString(this IRandomizer randomizer, int length = 10)
    {
        var randomChars = Enumerable.Range(0, length)
            .Select(x => (char)AsciiCharCodes[randomizer.Random(0, AsciiCharCodes.Length)]);

        return string.Join("", randomChars);
    }
}