using System;

namespace ExampleGame.Assets.BSP.Types
{
    class DispTri
    {
        [Flags]
        public enum Tags
        {
            SURFACE = 1,
            WALKABLE = 2,
            BUILDABLE = 4,
            SURFPROP1 = 8,
            SURFPROP2 = 16
        }

        public ushort tags;
    }
}
