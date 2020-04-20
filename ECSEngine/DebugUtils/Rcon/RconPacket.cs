using System.Collections.Generic;

namespace ECSEngine.DebugUtils.Rcon
{
    class RconPacket
    {
        public RconPacketType type;

        public Dictionary<string, string> data;

        public RconPacket(RconPacketType type, Dictionary<string, string> data)
        {
            this.type = type;
            this.data = data;
        }
    }
}
