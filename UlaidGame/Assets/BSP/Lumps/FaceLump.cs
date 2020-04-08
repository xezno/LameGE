using System.Collections.Generic;
using UlaidGame.Assets.BSP.Types;

namespace UlaidGame.Assets.BSP.Lumps
{
    class FaceLump : BaseLump
    {
        public List<Face> Contents { get; } = new List<Face>();
    }
}
