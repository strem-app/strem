using Strem.Core.Variables;

namespace Strem.UnitTests.Core.State;

public class VariablesTests
{
    [Theory]
    [InlineData("test", "context", "context", true)]
    [InlineData("test", "context", "", true)]
    [InlineData("test", "context", null, true)]
    [InlineData("test", "context", "context2", false)]
    public void should_correctly_identify_if_it_has_key(string variableName, string creationContext, string searchContext, bool shouldFind)
    {
        var creationEntry = new VariableEntry(variableName, creationContext);
        var variables = new Variables(new Dictionary<VariableEntry, string>
        {
            {creationEntry, "value"}
        });

        var searchEntry = new VariableEntry(variableName, searchContext);
        var hasMatch = variables.Has(searchEntry);
        Assert.Equal(shouldFind, hasMatch);
    }
    
    [Theory]
    [InlineData("test", "context", "context", true)]
    [InlineData("test", "context", "", true)]
    [InlineData("test", "context", null, true)]
    [InlineData("test", "context", "context2", false)]
    public void should_correctly_get_key(string variableName, string creationContext, string searchContext, bool shouldFindValue)
    {
        var expectedValue = "value";
        var creationEntry = new VariableEntry(variableName, creationContext);
        var variables = new Variables(new Dictionary<VariableEntry, string>
        {
            {creationEntry, expectedValue}
        });

        var searchEntry = new VariableEntry(variableName, searchContext);
        var actualValue = variables.Get(searchEntry);

        if (shouldFindValue)
        { Assert.Equal(expectedValue, actualValue); }
        else
        { Assert.NotEqual(expectedValue, actualValue); }
    }
}