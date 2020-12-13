using System.Collections.Generic;

namespace ExampleGame.Assets.BSP.Lumps
{
    public class Lump { }

    public class Lump<T> : Lump
    {
        public List<T> Contents { get; } = new List<T>();
    }
}
