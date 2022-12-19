using OBSWebsocketDotNet.Types.Events;

namespace Strem.OBS.Extensions;

public static class ObsVolumeExtensions
{
    const double LOG_RANGE_DB = 96.0;
    const double  LOG_OFFSET_DB = 6.0;

    const double  LOG_OFFSET_VAL = -0.77815125038364363;
    const double  LOG_RANGE_VAL = -2.00860017176191756;
    
    public static int SliderPercent(this InputVolumeChangedEventArgs args)
    {
        if (args.Volume.InputVolumeDb >= 0.0) { return 100; }
        if (args.Volume.InputVolumeDb <= -LOG_RANGE_DB) { return 0; }

        var percentage = (-Math.Log10(-args.Volume.InputVolumeDb + LOG_OFFSET_DB) - LOG_RANGE_VAL) / (LOG_OFFSET_VAL - LOG_RANGE_VAL) * 100.0;
        return (int)Math.Round(percentage);
    }
}