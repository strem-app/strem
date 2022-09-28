using System.Reactive.Disposables;
using System.Reactive.Linq;
using Obs.v5.WebSocket.Reactive;
using OBSWebsocketDotNet.Communication;
using OBSWebsocketDotNet.Types;
using Persistity.Encryption;
using Persistity.Extensions;
using Strem.Core.Extensions;
using Strem.Core.State;
using Strem.OBS.Plugin;
using Strem.OBS.Variables;

namespace Strem.OBS.Extensions;

public static class IObservableOBSWebSocketExtensions
{
    public const string NestedSceneItemSeparator = " => ";
    
    public static Task<(bool success, string message)> AttemptConnect(this IObservableOBSWebSocket obsClient, string host, string port, string password)
    {
        var websocketAddress = $"ws://{host}:{port}/api/websocket";
        var message = string.Empty;
        var success = false;
        var _subs = new CompositeDisposable();
        var taskCompletionSource = new TaskCompletionSource<(bool success, string message)>();

        void onConnected()
        {
            if(!taskCompletionSource.Task.IsCompleted)
            { taskCompletionSource.SetResult((true, string.Empty)); }
            _subs.Dispose();
        }

        void onDisconnected(ObsDisconnectionInfo obsDisconnectionInfo)
        {
            if(!taskCompletionSource.Task.IsCompleted)
            { taskCompletionSource.SetResult((false, obsDisconnectionInfo.DisconnectReason ?? "Could not connect to server")); }
            _subs.Dispose();
        }

        void onTimedOut()
        {
            if(!taskCompletionSource.Task.IsCompleted)
            { taskCompletionSource.SetResult((false, "Could not get response within reasonable time")); }
            _subs.Dispose();
        }
        
        try
        {
            obsClient.OnConnected
                .Subscribe(_ => onConnected())
                .AddTo(_subs);
            
            obsClient.OnDisconnected
                .Subscribe(onDisconnected)
                .AddTo(_subs);

            Observable.Interval(TimeSpan.FromSeconds(5))
                .Subscribe(_ => onTimedOut())
                .AddTo(_subs);
            
            obsClient.ConnectAsync(websocketAddress, password);
        }
        catch (Exception ex)
        {
            message = $"Connect Failed : {ex.Message}";
            taskCompletionSource.SetResult((false, message));
            _subs.Dispose();
        }

        return taskCompletionSource.Task;
    }
    
    public static async Task<(bool success, string message)> AttemptConnect(this IObservableOBSWebSocket client, IAppState appState, IEncryptor encryptor)
    {
        var host = appState.AppVariables.Get(OBSVars.Host);
        var port = appState.AppVariables.Get(OBSVars.Port);
        var password = string.Empty;

        if (appState.AppVariables.Has(OBSVars.Password))
        {
            var encryptedPassword = appState.AppVariables.Get(OBSVars.Password);
            password = encryptor.Decrypt(encryptedPassword);
        }

        return await client.AttemptConnect(host, port, password);
    }

    public static string[] GetAllSourceNames(this IObservableOBSWebSocket client)
    {
        return client.GetInputList()?.Select(x => x.InputName).ToArray() ?? Array.Empty<string>();
    }

    public static IReadOnlyCollection<string> GetAllSceneItems(this IObservableOBSWebSocket client, string sceneOrGroupName, bool isGroup = false, string prefix = "")
    {
        var allItems = new List<string>();
        IReadOnlyCollection<SceneItemDetails> currentItems;

        if (!isGroup)
        { currentItems = client.GetSceneItemList(sceneOrGroupName); }
        else
        { currentItems = (client as DebugOBSSocket).GetGroupItemList(sceneOrGroupName); }
        
        foreach (var sceneItem in currentItems)
        {
            if (sceneItem.SourceKind == null)
            {
                allItems.Add(sceneItem.SourceName);
                var newPrefix = string.IsNullOrEmpty(prefix)
                    ? sceneItem.SourceName
                    : $"{prefix}{NestedSceneItemSeparator}{sceneItem.SourceName}";
                
                var nestedSceneItems = GetAllSceneItems(client, sceneItem.SourceName, true, newPrefix);
                allItems.AddRange(nestedSceneItems);
                continue;
            }

            if (string.IsNullOrEmpty(prefix))
            { allItems.Add(sceneItem.SourceName); }
            else
            { allItems.Add($"{prefix}{NestedSceneItemSeparator}{sceneItem.SourceName}"); }
        }
        
        return allItems;
    }
}