namespace Strem.Twitch.Extensions;

public static class TwitchCurrencyExtension
{
    // TODO: Maybe better way to calculate the major currency
    public static float ProcessTwitchCurrency(this int minorCurrency, int decimalPlaces)
    {
        var divider = int.Parse("1".PadRight(decimalPlaces + 1, '0'));
        return (float)minorCurrency/divider;
    }
}