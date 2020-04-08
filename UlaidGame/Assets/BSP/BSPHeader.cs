namespace UlaidGame.Assets.BSP
{
    struct BSPHeader
    {
        public byte[] magicNumber; // VBSP (Valve BSP)
        public int version; // 20 for GMod maps
        public BSPLumpInfo[] lumpDirectory; // Directory of lumps
        public int mapRevision; // Map iteration
    }
}
