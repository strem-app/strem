using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Obs.v5.WebSocket.Reactive;
using OBSWebsocketDotNet.Types;

namespace Strem.OBS.Plugin;

public class DebugOBSSocket : ObservableOBSWebSocket
{
    public IReadOnlyCollection<SceneItemDetails> GetGroupItemList(string sceneName)
    {
        var payload = new JObject() { { nameof(sceneName), (JToken)sceneName } };
        var response = SendRequest(nameof(GetGroupSceneItemList), payload);
        return response["sceneItems"].Select(x => new SceneItemDetails(x as JObject)).ToArray();
    }
}