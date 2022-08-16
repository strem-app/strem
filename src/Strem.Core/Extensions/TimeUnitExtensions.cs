using Strem.Core.Types;

namespace Strem.Core.Extensions;

public static class TimeUnitExtensions
{
    public static TimeSpan ToTimeSpan(this TimeUnitType timeUnitType, int unitValue)
    {
        return timeUnitType switch
        {
            TimeUnitType.Minutes => TimeSpan.FromMinutes(unitValue),
            TimeUnitType.Hours => TimeSpan.FromHours(unitValue),
            TimeUnitType.Days => TimeSpan.FromDays(unitValue),
            _ => TimeSpan.FromSeconds(unitValue)
        };
    }
}