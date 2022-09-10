using Newtonsoft.Json;
using Strem.Core.Variables;

namespace Strem.UnitTests.SanityTests;

public class VariableEntryTests
{
    [Fact]
    public void should_work_correctly_for_keys()
    {
        var dictionary = new Dictionary<VariableEntry, int>();
        var original = new VariableEntry("test", "context-1");
        var matching = new VariableEntry("test", "context-1");
        var notMatching = new VariableEntry("ab", "cd");
        var notMatching2 = new VariableEntry("ab", null);
        
        dictionary.Add(original, 1);
        Assert.True(dictionary.ContainsKey(original));
        Assert.True(dictionary.ContainsKey(matching));
        Assert.False(dictionary.ContainsKey(notMatching));
        Assert.False(dictionary.ContainsKey(notMatching2));

        var result = dictionary[matching];
        Assert.Equal(1, result);
    }
}