using Strem.Core.Types;

namespace Strem.Core.Extensions;

public static class IntExtensions
{
    public static bool MatchesOperator(this int a, int b, OperatorType operatorType)
    {
        return operatorType switch
        {
            OperatorType.NotEquals => a != b,
            OperatorType.GreaterThan => a > b,
            OperatorType.GreaterThanEqual => a >= b,
            OperatorType.LessThan => a < b,
            OperatorType.LessThanEqual => a <= b,
            _ => a == b
        };
    }
}