using System.Collections.Generic;

namespace ECSEngine.DebugUtils.Rcon
{
    class RconPacket
    {
        public RconPacketOrigin origin;
        public RconPacketType type;

        public Dictionary<string, string> data;

        public RconPacket(RconPacketOrigin origin, RconPacketType type, Dictionary<string, string> data)
        {
            this.origin = origin;
            this.type = type;
            this.data = data;
        }
    }
}
