using System.Collections.Generic;
using UlaidGame.Assets.BSP.Types;

namespace UlaidGame.Assets.BSP.Lumps
{
    class EdgeLump : BaseLump
    {
        public List<Edge> Contents { get; } = new List<Edge>();
    }
}