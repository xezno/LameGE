using Engine.MathUtils;
using System.IO;

namespace Ulaid.Assets.BSP
{
    public static class BinaryReaderExt
    {
        public static Vector3 ReadVector3(this BinaryReader binaryReader)
        {
            return new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(),
                binaryReader.ReadSingle());
        }
    }
}
