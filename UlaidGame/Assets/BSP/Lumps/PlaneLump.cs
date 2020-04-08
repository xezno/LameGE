using System.Collections.Generic;
using UlaidGame.Assets.BSP.Types;

namespace UlaidGame.Assets.BSP.Lumps
{
    class PlaneLump : BaseLump
    {
        public List<Plane> Contents { get; } = new List<Plane>();
    }
}
