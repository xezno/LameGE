﻿using Engine.Utils.MathUtils;
using System.Numerics;

namespace ExampleGame.Assets.BSP.Types
{
    struct DispNeighbor
    {
        // ???
    }

    struct DispCornerNeighbor
    {
        // ???
    }

    class DispInfo
    {
        public Vector3 startPosition;
        public int dispVertStrt;
        public int dispTriStart;
        public int power;
        public int minTess;
        public float smoothingAngle;
        public int contents;
        public ushort mapFace;
        public int lightmapAlphaStart;
        public int lightmapSamplePositionStart;
        public DispNeighbor[] dispNeighbor; // 4
        public DispCornerNeighbor[] dispCornerNeighbor; // 4
    }
}
