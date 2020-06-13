using Engine.Utils.DebugUtils;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Ulaid.Craft.Packets.Clientbound.ServerListPing;
using Ulaid.Craft.Packets.Serverbound.Handshake;
using Ulaid.Craft.Packets.Serverbound.ServerListPing;
using Ulaid.Craft.Types;

namespace Ulaid.Craft.Client
{
    public class TestLogin
    {
        private TcpClient tcpClient;
        private Thread receiveThread;
        private NetworkStream networkStream;

        private string serverIp;
        private ushort serverPort;

        public TestLogin(string serverIp, ushort serverPort, string playerName, string playerPassword) 
        { 
            tcpClient = new TcpClient(serverIp, serverPort);
            this.serverIp = serverIp;
            this.serverPort = serverPort;

            networkStream = tcpClient.GetStream();
        }

        public void Run()
        {
            if (!tcpClient.Connected)
            {
                Logging.Log("Client not connected! :(");
            }

            // Handshake packet
            var handshakePacket = new Handshake(serverIp, serverPort, nextState: 2);
            SendPacket(handshakePacket);
            // Login start packet
            var loginStart = new LoginStart(playerName);
            SendPacket(loginStart);
            
            receiveThread = new Thread(ReceiveData);
            receiveThread.Start();
        }

        private void ReceiveData()
        {
            while (tcpClient.Connected)
            {
                int currentAvail;
                do
                {
                    currentAvail = tcpClient.Available;
                } while (currentAvail == tcpClient.Available);

                // Read packet length
                var packetLength = new VarInt(networkStream);

                Logging.Log($"Got packet of length {packetLength}");
                
                // Wait until more is available, then read more
                do
                {
                    currentAvail = tcpClient.Available;
                } while (currentAvail < packetLength);

                // Read packet into buffer
                byte[] data = new byte[packetLength];
                networkStream.Read(data, 0, packetLength);
                MemoryStream memoryStream = new MemoryStream(data);
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                
                // Read packet id
                var packetId = new VarInt(memoryStream);

                switch ((PacketTypes.Clientbound)packetId.Value)
                {
                    case PacketTypes.Clientbound.Response:
                        HandleResponsePacket(new Response().GetData(ref binaryReader));
                        break;
                    case PacketTypes.Clientbound.Pong:
                        HandlePongPacket();
                        break;
                    default:
                        throw new Exception("how");
                }
            }
        }

        private void HandleResponsePacket(PacketField packetField)
        {
            // Handle Response packet
            if (packetField.FieldType == typeof(ServerListPingResponse))
            {
                var serverListPingResponse = (ServerListPingResponse)packetField.Value;

                Logging.Log($"Server: {serverListPingResponse.Description} ({serverListPingResponse.Players.Online} / {serverListPingResponse.Players.Max})");
                
                Logging.Log($"Game version: {serverListPingResponse.Version.Name} ({serverListPingResponse.Version.Protocol})");
                Logging.Log($"Are we compatible? {serverListPingResponse.Version.Protocol == ProtocolInfo.VersionInfo.Version}");
            }
            else
            {
                throw new Exception("how");
            }

            // Send back a ping packet
            var pingPacket = new Ping();
            SendPacket(pingPacket);
        }

        private void HandlePongPacket()
        {
            Logging.Log("Server pong");
        }

        private void SendPacket(ServerboundPacket packet)
        { 
            // Packet format:
            // Length - varint
            // Packet id - varint
            // Data - byte[]

            var packetIdVarInt = new VarInt((int)packet.PacketId);

            PacketWriter packetWriter = new PacketWriter();

            packet.SetData(ref packetWriter);

            var dataLength = packetWriter.Position;
            packetWriter.Position = 0;

            packetWriter.Write(new VarInt(dataLength + packetIdVarInt.RawValue.Length)); // Packet length
            packetWriter.Write(new VarInt(packetIdVarInt)); // Packet ID
            
            tcpClient.GetStream().Write(packetWriter.Buffer.ToArray(), 0, packetWriter.Length);
        }
    }
}
