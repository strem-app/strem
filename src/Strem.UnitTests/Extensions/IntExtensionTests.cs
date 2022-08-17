using Strem.Core.Extensions;
using Strem.Core.Types;

namespace Strem.UnitTests.Extensions;

public class IntExtensionTests
{
    [Theory]
    [InlineData(1,1, OperatorType.Equals, true)]
    [InlineData(1,0, OperatorType.Equals, false)]
    [InlineData(1,123, OperatorType.Equals, false)]
    [InlineData(1,123, OperatorType.NotEquals, true)]
    [InlineData(1,1, OperatorType.NotEquals, false)]
    [InlineData(0,0, OperatorType.NotEquals, false)]
    [InlineData(0,0, OperatorType.GreaterThan, false)]
    [InlineData(0,1, OperatorType.GreaterThan, false)]
    [InlineData(1,0, OperatorType.GreaterThan, true)]
    [InlineData(1,0, OperatorType.GreaterThanEqual, true)]
    [InlineData(0,0, OperatorType.GreaterThanEqual, true)]
    [InlineData(0,1, OperatorType.GreaterThanEqual, false)]
    [InlineData(0,1, OperatorType.LessThanEqual, true)]
    [InlineData(0,0, OperatorType.LessThanEqual, true)]
    [InlineData(123,0, OperatorType.LessThanEqual, false)]
    [InlineData(1,0, OperatorType.LessThanEqual, false)]
    [InlineData(1,0, OperatorType.LessThan, false)]
    [InlineData(0,0, OperatorType.LessThan, false)]
    [InlineData(0,1, OperatorType.LessThan, true)]
    public void should_match_correctly(int valueA, int valueB, OperatorType operatorType, bool expectedMatch)
    {
        var actualMatch = valueA.MatchesOperator(valueB, operatorType);
        Assert.Equal(expectedMatch, actualMatch);
    }
}