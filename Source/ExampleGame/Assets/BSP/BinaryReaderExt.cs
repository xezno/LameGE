using Engine.Utils.MathUtils;
using System.IO;

namespace ExampleGame.Assets.BSP
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
