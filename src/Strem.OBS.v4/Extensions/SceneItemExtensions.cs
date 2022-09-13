
using Obs.v4.WebSocket.Types;
using Strem.Core.Extensions;

namespace Strem.OBS.v4.Extensions;

public static class SceneItemExtensions
{
    public static IEnumerable<string> GetAllSourceNames(this IEnumerable<SceneItem> sceneItems)
    {
        var sourceNames = new List<string>();
        foreach (var sceneItem in sceneItems)
        {
            sourceNames.Add(sceneItem.SourceName);
            sceneItem.GroupChildren?
                .GetAllSourceNames()
                .ForEach(sourceNames.Add);
        }
        return sourceNames;
    }
}