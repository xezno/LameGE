using Engine.Common.MathUtils;
using System.IO;

namespace Example.Assets.BSP
{
    public static class BinaryReaderExt
    {
        public static Vector3f ReadVector3(this BinaryReader binaryReader)
        {
            return new Vector3f(binaryReader.ReadSingle(), binaryReader.ReadSingle(),
                binaryReader.ReadSingle());
        }
    }
}
