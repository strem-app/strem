using Newtonsoft.Json;
using Strem.Core.Variables;

namespace Strem.UnitTests.SanityTests;

public class VariableEntryConvertorSanityTests
{
    [Theory]
    [InlineData("test1", "context1", "value1")]
    [InlineData("test1", "context2", "value2")]
    [InlineData("test1", null, "value3")]
    public void should_serialize_and_deserialize_dictionary_correctly(string name, string context, string value)
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new VariableEntryConvertor() }
        };

        var expected = new KeyValuePair<VariableEntry, string>(new VariableEntry(name, context), value);
        var output = JsonConvert.SerializeObject(expected);
        Assert.NotNull(output);
        Assert.NotEmpty(output);

        var actual = JsonConvert.DeserializeObject<KeyValuePair<VariableEntry, string>>(output);
        Assert.Equal(actual.Key, expected.Key);
        Assert.Equal(actual.Value, expected.Value);
    }
}