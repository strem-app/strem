using Newtonsoft.Json;
using Strem.Core.State;

namespace Strem.UnitTests.SanityTests;

public class VariableDictionaryConvertorSanityTests
{
    [Fact]
    public void should_serialize_and_deserialize_dictionary_correctly()
    {
        var expectedDictionary = new Dictionary<VariableEntry, string>();
        expectedDictionary.Add(new VariableEntry("test1", "context1"), "value1");
        expectedDictionary.Add(new VariableEntry("test2", "context2"), "value2");

        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new VariableDictionaryConvertor() }
        };

        var output = JsonConvert.SerializeObject(expectedDictionary);
        Assert.NotNull(output);
        Assert.NotEmpty(output);

        var actualDictionary = JsonConvert.DeserializeObject<Dictionary<VariableEntry, string>>(output);
        Assert.NotNull(actualDictionary);
        Assert.NotEmpty(actualDictionary);
        Assert.Equal(expectedDictionary, actualDictionary);

    }
}