using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using ECSEngine.DebugUtils;
using ECSEngine.MathUtils;
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

        private DebugCommand[] commands =
        {
            new DebugCommand(new [] { "loadLevel" }, "Load a level", "gm_construct.bsp"),
            new DebugCommand(new [] { "reload" }, "Reload all content", ""),
            new DebugCommand(new [] { "loadScript" }, "Load a script", ""),
            new DebugCommand(new [] { "anvil" }, "Display Anvil status", ""),
            new DebugCommand(new [] { "reloadAnvil" }, "Reload Anvil", ""),
            new DebugCommand(new [] { "loadScriptAnvil" }, "Load Anvil script", ""),
            new DebugCommand(new [] { "cheatsEnabled" }, "Toggle cheats", "1"),
            new DebugCommand(new [] { "rconPassword" }, "Set rcon password", "Schlinx"),
            new DebugCommand(new [] { "rconEnabled" }, "Toggle rcon", "1"),
            new DebugCommand(new [] { "renderResX" }, "Set render resolution (X)", "1280"),
            new DebugCommand(new [] { "renderResY" }, "Set render resolution (Y)", "720"),
            new DebugCommand(new [] { "fullscreenEnabled" }, "Toggle fullscreen", "0"),
            new DebugCommand(new [] { "vsyncEnabled" }, "Toggle vsync", "0"),
            new DebugCommand(new [] { "fpsLimit" }, "Set fps limit (-1 to disable)", "75"),
            new DebugCommand(new [] { "editorEnablde" }, "Toggle editor", "0")
        };

        public RemoteConsoleManager()
        {
            if (!GameSettings.Default.rconEnabled)
                return;

            socketServer = new WebSocketServer($"ws://0.0.0.0:{port}");
            socketServer.SupportedSubProtocols = new[] { "ulaidRcon" };
            socketServer.ListenerSocket.NoDelay = false;
            socketServer.Start(InitConnection);

            FleckLog.LogAction = CustomFleckLog;

        }

        private void CustomFleckLog(LogLevel level, string message, Exception ex)
        {
            if (level != LogLevel.Error) // We don't care about anything that isn't an error (currently).
                return;

            var logSeverity = Logging.Severity.Low;
            switch (level)
            {
                case LogLevel.Warn:
                    logSeverity = Logging.Severity.Medium;
                    break;
                case LogLevel.Error:
                    logSeverity = Logging.Severity.High;
                    break;
                case LogLevel.Debug:
                case LogLevel.Info:
                default:
                    break;
            }

            Logging.Log(message, logSeverity);
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

            if (rconPacket.type == RconPacketType.Handshake)
            {
                if (rconPacket.data["password"] == GameSettings.Default.rconPassword)
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

            if (!authenticated) return;

            if (rconPacket.type == RconPacketType.Input)
            {
                Logging.Log($"Received input {rconPacket.data["input"]}");
            }
            else if (rconPacket.type == RconPacketType.InputInProgress)
            {
                SendSuggestions(rconPacket.data["input"]);
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
            foreach (var historyEntry in Logging.LogHistory)
            {
                SendPacket(new RconPacket(RconPacketOrigin.Server, RconPacketType.LogHistory, new Dictionary<string, string>()
                {
                    { "timestamp", historyEntry.timestamp.ToString("T") },
                    { "stackTrace", historyEntry.stackTrace.ToString() },
                    { "str", historyEntry.str },
                    { "severity", historyEntry.severity.ToString().ToLower() }
                }));
            }
        }

        private void SendSuggestions(string input)
        {
            List<DebugCommand> suggestions = new List<DebugCommand>();
            int count = 0;
            foreach (var command in commands)
            {
                if (command.aliases[0].IndexOf(input, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    suggestions.Add(command);
                    count++;
                }

                if (count > 5)
                    break;
            }

            if (suggestions.Count == 0)
            {
                SendPacket(new RconPacket(RconPacketOrigin.Server, RconPacketType.Suggestions, new Dictionary<string, string>() { }));
            }
            else
            {
                SendPacket(new RconPacket(RconPacketOrigin.Server, RconPacketType.Suggestions, new Dictionary<string, string>()
                {
                    { "suggestions", JsonConvert.SerializeObject(suggestions) }
                }));
            }
        }

        public void SendDebugLog(DateTime timestamp, StackTrace stackTrace, string log, Logging.Severity severity)
        {
            if (connected && authenticated)
            {
                SendPacket(new RconPacket(RconPacketOrigin.Server, RconPacketType.Response, new Dictionary<string, string>()
                {
                    { "timestamp", timestamp.ToString("T") },
                    { "stackTrace", stackTrace.ToString() },
                    { "str", log },
                    { "severity", severity.ToString().ToLower() }
                }));
            }
        }
    }
}
