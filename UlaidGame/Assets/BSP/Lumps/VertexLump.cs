using System.Collections.Generic;
using ECSEngine.MathUtils;

namespace UlaidGame.Assets.BSP.Lumps
{
    class VertexLump : BaseLump
    {
        public List<Vector3> Contents { get; } = new List<Vector3>();
    }
}
