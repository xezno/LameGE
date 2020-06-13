using System.IO;

namespace Ulaid.Craft.Packets.Serverbound.Login
{
    public class LoginStart : ServerboundPacket
    {
        public override PacketTypes.Serverbound PacketId => PacketTypes.Serverbound.LoginStart;

        private readonly string playerName;

        public LoginStart(string playerName)
        {
            this.playerName = playerName;
        }

        public override void SetData(ref PacketWriter packetWriter)
        {
            // Name (up to 16 chars)
            if (playerName.Length > 16)
            {
                throw new System.Exception("Username too long");
            }

            if (playerName.Length < 3)
            {
                throw new System.Exception("Username too short?");
            }

            packetWriter.Write(playerName);
        }
    }
}
