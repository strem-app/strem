using OBSWebsocketDotNet.Types.Events;

namespace Strem.OBS.Extensions;

public static class ObsVolumeExtensions
{
    const double LOG_RANGE_DB = 96.0;
    const double  LOG_OFFSET_DB = 6.0;

    const double  LOG_OFFSET_VAL = -0.77815125038364363;
    const double  LOG_RANGE_VAL = -2.00860017176191756;
    
    public static int DecibelToPercent(this float decibelVolume)
    {
        if (decibelVolume >= 0.0) { return 100; }
        if (decibelVolume <= -LOG_RANGE_DB) { return 0; }

        var percentage = (-Math.Log10(-decibelVolume + LOG_OFFSET_DB) - LOG_RANGE_VAL) / (LOG_OFFSET_VAL - LOG_RANGE_VAL) * 100.0;
        return (int)Math.Round(percentage);
    }
    
    public static float PercentToDecibel(this int percentage)
    {
        if (percentage >= 100) { return 0; }
        if (percentage <= 0) { return -100; }

        var normalized = (float)percentage / 100;
        var decibels = -(LOG_RANGE_DB + LOG_OFFSET_DB) * Math.Pow((LOG_RANGE_DB + LOG_OFFSET_DB) / LOG_OFFSET_DB, -normalized) + LOG_OFFSET_DB;
        return (float)decibels;
    }
}