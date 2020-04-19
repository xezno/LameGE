using ECSEngine.DebugUtils;
using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ECSEngine.Managers
{
    public sealed class RconManager : Manager<RconManager>
    {
        #region Fields
        private bool connected;
        private bool authenticated;
        private IWebSocketConnection localSocket;

        private List<DebugCommand> debugCommands = new List<DebugCommand>();
        #endregion

        #region Constructor
        public RconManager()
        {
            // WARNING! DO NOT LOG IN HERE!

            if (!GameSettings.Default.rconEnabled)
                return;

            FleckLog.LogAction = CustomFleckLog;

            var socketServer = new WebSocketServer($"ws://0.0.0.0:{GameSettings.Default.rconPort}")
            {
                SupportedSubProtocols = new[] { "ulaidRcon" },
                ListenerSocket =
                {
                    NoDelay = true
                }
            };

            socketServer.Start(InitConnection);
        }
        #endregion

        #region Command Registry
        public void RegisterCommand<T>(string name, string description, Func<T> getter, Action<T> setter)
        {
            debugCommands.Add(new DebugCommand<T>(name, description, getter, setter));
        }

        public void RegisterCommand<T>(string name, string description, Action method)
        {
            // debugCommands.Add(new DebugCommand<T>(name, description));
            throw new NotImplementedException();
        }
        #endregion

        #region Fleck / Websocket Events
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
            }

            Logging.Log(message, logSeverity);
            if (ex != null)
                Logging.Log(ex.ToString(), Logging.Severity.High);
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
            Logging.Log($"Remote console connection started: {localSocket.ConnectionInfo.ClientIpAddress}:{localSocket.ConnectionInfo.ClientPort}");
            connected = true;
        }

        private void OnClose()
        {
            Logging.Log("Remote console connection closed");
            connected = false;
        }

        private void OnMessage(string message)
        {
            var rconPacket = Newtonsoft.Json.JsonConvert.DeserializeObject<RconPacket>(message);

            switch (rconPacket.type)
            {
                case RconPacketType.Handshake:
                    HandleHandshake(rconPacket);
                    break;
                case RconPacketType.Authenticate:
                    HandleAuthentication(rconPacket);
                    break;
            }

            if (!authenticated) return;

            switch (rconPacket.type)
            {
                case RconPacketType.Input:
                    HandleInput(rconPacket);
                    break;
                case RconPacketType.InputInProgress:
                    HandleInputInProgress(rconPacket);
                    break;
            }
        }
        #endregion

        #region Packet Handlers
        private void HandleHandshake(RconPacket rconPacket)
        {
            Logging.Log("Received handshake from client.");
            if (string.IsNullOrEmpty(GameSettings.Default.rconPassword))
            {
                Logging.Log("Rcon authentication is disabled! Please enter a password in GameSettings if this is incorrect", Logging.Severity.Medium);

                if (localSocket.ConnectionInfo.ClientIpAddress != "127.0.0.1")
                {
                    Logging.Log("Rcon connection attempt from non-local machine was blocked.", Logging.Severity.Medium);
                    return;
                }

                authenticated = true;
                SendLogHistory();
            }
            else
            {
                // We need authentication!
                SendPacket(RconPacketType.RequestAuth, new Dictionary<string, string>());
            }
        }

        private void HandleAuthentication(RconPacket rconPacket)
        {
            if (rconPacket.data["password"] == GameSettings.Default.rconPassword)
            {
                authenticated = true;
                SendLogHistory();
            }
            else
            {
                Logging.Log("Rcon password was incorrect");
                SendPacket(RconPacketType.Error, new Dictionary<string, string>()
                {
                    { "errorMessage", "Password incorrect :(" }
                });
                localSocket.Close();
            }
        }

        private void HandleInput(RconPacket rconPacket)
        {
            Logging.Log($"Received input {rconPacket.data["input"]}");
        }

        private void HandleInputInProgress(RconPacket rconPacket)
        {
            SendSuggestions(rconPacket.data["input"]);
        }
        #endregion

        #region Packet Senders
        private void SendPacket(RconPacketType type, Dictionary<string, string> data)
        {
            var str = JsonConvert.SerializeObject(new RconPacket(RconPacketOrigin.Server, type, data));
            var bytes = Encoding.UTF8.GetBytes(str);
            localSocket.Send(bytes);
        }

        private void SendLogHistory()
        {
            var entries = new List<Dictionary<string, string>>();
            foreach (var entry in Logging.LogHistory)
            {
                entries.Add(GetResultDictionary(entry.timestamp, entry.stackTrace, entry.str, entry.severity));
            }
            SendPacket(RconPacketType.LogHistory, new Dictionary<string, string>()
            {
                { "entries", JsonConvert.SerializeObject(entries) }
            });
        }

        private void SendSuggestions(string input)
        {
            List<DebugCommand> suggestions = new List<DebugCommand>();
            int count = 0;
            foreach (var command in debugCommands)
            {
                if (command.MatchSuggestion(input))
                {
                    suggestions.Add(command);
                    count++;
                }

                if (count > 5)
                    break;
            }

            if (suggestions.Count == 0)
            {
                SendPacket(RconPacketType.Suggestions, new Dictionary<string, string>() { });
            }
            else
            {
                SendPacket(RconPacketType.Suggestions, new Dictionary<string, string>()
                {
                    { "suggestions", JsonConvert.SerializeObject(suggestions) }
                });
            }
        }

        public void SendDebugLog(DateTime timestamp, StackTrace stackTrace, string log, Logging.Severity severity)
        {
            if (connected && authenticated)
            {
                SendPacket(RconPacketType.Response, GetResultDictionary(timestamp, stackTrace, log, severity));
            }
        }

        private Dictionary<string, string> GetResultDictionary(DateTime timestamp, StackTrace stackTrace, string log,
            Logging.Severity severity)
        {
            return new Dictionary<string, string>()
            {
                {"timestamp", timestamp.ToString("T")},
                {"stackTrace", stackTrace.ToString()},
                {"str", log},
                {"severity", severity.ToString().ToLower()}
            };
        }
        #endregion

        #region Debug commands

        public string Find(string str)
        {
            List<DebugCommand> suggestions = new List<DebugCommand>();
            foreach (var command in debugCommands)
            {
                if (command.MatchSuggestion(str))
                    suggestions.Add(command);
            }

            return JsonConvert.SerializeObject(suggestions);
        }
        #endregion
    }
}
