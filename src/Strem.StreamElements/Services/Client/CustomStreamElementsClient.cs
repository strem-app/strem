using System.Reflection;
using WebSocket4Net;

namespace Strem.StreamElements.Services.Client;

public class CustomStreamElementsClient : StreamElementsNET.Client
{
    public bool IsConnected { get; protected set; }

    public CustomStreamElementsClient() : base(null)
    {
        // Hack for now
        OnConnected += (sender, args) => IsConnected = true;
        OnDisconnected += (sender, args) => IsConnected = false;
    }
    
    public void Connect(string jwt)
    {
        // MANUALLY OVERRIDE THE JWT, raised github issue for this
        var jwtField = typeof(StreamElementsNET.Client).GetField("jwt", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
        jwtField.SetValue(this, jwt);
        
        Connect();
    }
}