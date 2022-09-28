using OBSWebsocketDotNet.Types;
using Strem.OBS.Types;

namespace Strem.OBS.Extensions;

public static class SceneItemDetailsExtensions
{
    public static bool IsCaptureSource(this SceneItemDetails sceneItemDetails)
    { return sceneItemDetails.SourceKind.Contains(SourceKindType.CaptureSource); }
    
    public static bool IsCaptureSource(this InputBasicInfo inputDetails)
    { return inputDetails.InputKind.Contains(SourceKindType.CaptureSource); }
}