using System.Collections.Generic;

namespace Example.Assets.BSP.Lumps
{
    public class Lump { }

    public class Lump<T> : Lump
    {
        public List<T> Contents { get; } = new List<T>();
    }
}
