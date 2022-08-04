using Strem.Core.Types;

namespace Strem.Core.Extensions;

public static class TimeUnitExtensions
{
    public static TimeSpan ToTimeSpan(this TimeUnit timeUnit, int unitValue)
    {
        return timeUnit switch
        {
            TimeUnit.Minutes => TimeSpan.FromMinutes(unitValue),
            TimeUnit.Hours => TimeSpan.FromHours(unitValue),
            TimeUnit.Days => TimeSpan.FromDays(unitValue),
            _ => TimeSpan.FromSeconds(unitValue)
        };
    }
}