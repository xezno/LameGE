using System.Text;
using System.Collections.Generic;
using ECSEngine.Debug;
using Fleck;
using Newtonsoft.Json;

namespace ECSEngine.Managers
{
    public sealed class RemoteConsoleManager : Manager<RemoteConsoleManager>
    {
        private int port = 42069;
        private WebSocketServer socketServer;
        private bool connected;
        private bool authenticated;
        private IWebSocketConnection localSocket;

        public RemoteConsoleManager()
        {
            if (!GameSettings.Default.rconEnabled)
                return;

            socketServer = new WebSocketServer($"ws://0.0.0.0:{port}");
            socketServer.SupportedSubProtocols = new[] { "ulaidRcon" };
            socketServer.ListenerSocket.NoDelay = true;
            socketServer.Start(InitConnection);
        }

        private void InitConnection(IWebSocketConnection socket)
        {
            localSocket = socket;
            socket.OnOpen = OnOpen;
            socket.OnClose = OnClose;
            socket.OnMessage = OnMessage;
        }

        private void OnOpen()
        {
            Logging.Log("Remote console connection started");
            connected = true;
        }

        private void OnClose()
        {
            Logging.Log("Remote console connection closed");
            connected = false;
        }

        private void OnMessage(string message)
        {
            // Logging.Log($"Remote Console message: {message}");
            var rconPacket = Newtonsoft.Json.JsonConvert.DeserializeObject<RconPacket>(message);

            // Logging.Log($"Rcon packet info: {rconPacket.origin} / {rconPacket.type}");

            if (rconPacket.data["password"].Substring(0, rconPacket.data["password"].Length - 1) == GameSettings.Default.rconPassword)
            {
                authenticated = true;
                // Send log history
                SendLogHistory();
            }
            else
            {
                Logging.Log("Rcon password was incorrect");
                SendPacket(new RconPacket(RconPacketOrigin.Server, RconPacketType.Error, new Dictionary<string, string>()
                {
                    { "errorMessage", "Password incorrect :(" }
                }));
                localSocket.Close();
            }
        }

        private void SendPacket(RconPacket packet)
        {
            var str = JsonConvert.SerializeObject(packet);
            var bytes = Encoding.UTF8.GetBytes(str);
            localSocket.Send(bytes);
        }

        private void SendLogHistory()
        {
            foreach (var kvp in Logging.LogHistory)
            {
                SendPacket(new RconPacket(RconPacketOrigin.Server, RconPacketType.LogHistory, new Dictionary<string, string>()
                {
                    { "logString", kvp.Key },
                    { "severity", kvp.Value.ToString().ToLower() }
                }));
            }
        }

        public void SendDebugLog(string log, Logging.Severity severity)
        {
            if (connected && authenticated)
            {
                SendPacket(new RconPacket(RconPacketOrigin.Server, RconPacketType.Response, new Dictionary<string, string>()
                {
                    { "logString", log },
                    { "severity", severity.ToString().ToLower() }
                }));
            }
        }
    }
}
