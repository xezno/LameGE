using System.IO;

namespace Ulaid.Craft.Packets.Clientbound.ServerListPing
{
    public class Pong : ClientboundServerListPingPacket
    {
        public override PacketTypes.Clientbound PacketId => PacketTypes.Clientbound.Pong;

        public override PacketField GetData(ref BinaryReader binaryReader)
        {
            throw new System.NotImplementedException();
        }
    }
}
