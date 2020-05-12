namespace Ulaid.Assets.BSP.Types
{
    class Face
    {
        public ushort planeNumber;
        public byte side;
        public bool onNode;
        public int firstSurfEdge;
        public short numSurfEdges;
        public short texInfo;
        public short dispInfo;
        public short surfaceFogVolumeId;
        public byte[] styles;
        public int lightmapOffset;
        public float area;
        public int[] lightmapTextureMinsInLuxels;
        public int[] lightmapTextureSizeInLuxels;
        public int origFace;
        public ushort numPrims;
        public ushort firstPrimId;
        public uint smoothingGroups;
    }
}
