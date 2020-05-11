using Engine.DebugUtils;
using Engine.MathUtils;
using System.IO;
using System.Linq;
using Ulaid.Assets.BSP.Lumps;
using Ulaid.Assets.BSP.Types;

namespace Ulaid.Assets.BSP
{
    class BSPLoader
    {
        private BSPHeader header;
        public Lump[] Lumps { get; } = new Lump[64];

        public BSPLoader(string path)
        {
            using var streamReader = new StreamReader(path);
            using var binaryReader = new BinaryReader(streamReader.BaseStream);
            Logging.Log($"Reading BSP file {path}");

            ReadFile(binaryReader);
        }

        private void ReadFile(BinaryReader binaryReader)
        {
            ReadHeader(binaryReader);
            ReadLumps(binaryReader);
        }

        private void ReadHeader(BinaryReader binaryReader)
        {
            // Read header
            header = new BSPHeader
            {
                magicNumber = binaryReader.ReadBytes(4),
                version = binaryReader.ReadByte(),
                lumpDirectory = new BSPLumpInfo[64]
            };

            // Skip next 3 bytes
            binaryReader.ReadBytes(3);

            if (!header.magicNumber.SequenceEqual(new[] { (byte)'V', (byte)'B', (byte)'S', (byte)'P' }))
                Logging.Log("Not a valid BSP file!", Logging.Severity.High);

            if (header.version != 20)
                Logging.Log($"BSP version {header.version} is unsupported; will try to parse map anyway.", Logging.Severity.Medium);

            // Parse lumps info - assume 64 lumps are present
            for (int i = 0; i < 64; ++i)
            {
                header.lumpDirectory[i] = new BSPLumpInfo
                {
                    offset = binaryReader.ReadInt32(),
                    length = binaryReader.ReadInt32(),
                    formatVersion = binaryReader.ReadInt32(),
                    identityCode = binaryReader.ReadBytes(4)
                };
            }

            // Get map revision
            header.mapRevision = binaryReader.ReadInt32();
            Logging.Log($"BSP file has map revision {header.mapRevision}");
        }

        private void ReadLumps(BinaryReader binaryReader)
        {
            for (int i = 0; i < 64; ++i)
            {
                var lump = header.lumpDirectory[i];
                var lumpType = (BspLumpType)i;
                Logging.Log($"{lumpType} at {lump.offset} {lump.length}");
                if (lump.length > 0)
                {
                    ReadLump(binaryReader, lump, lumpType);
                }
            }
        }

        private void ReadLump(BinaryReader binaryReader, BSPLumpInfo lump, BspLumpType lumpType)
        {
            var startOffset = binaryReader.BaseStream.Position;
            binaryReader.BaseStream.Seek(lump.offset, SeekOrigin.Begin);

            // handle lump based on lumptype given
            switch (lumpType)
            {
                case BspLumpType.LumpPlanes:
                    Lumps[(int)lumpType] = ReadPlaneLump(lump, binaryReader);
                    break;
                case BspLumpType.LumpVertexes:
                    Lumps[(int)lumpType] = ReadVertexesLump(lump, binaryReader);
                    break;
                case BspLumpType.LumpEdges:
                    Lumps[(int)lumpType] = ReadEdgesLump(lump, binaryReader);
                    break;
                case BspLumpType.LumpSurfEdges:
                    Lumps[(int)lumpType] = ReadSurfEdgesLump(lump, binaryReader);
                    break;
                case BspLumpType.LumpFaces:
                    Lumps[(int)lumpType] = ReadFacesLump(lump, binaryReader);
                    break;
                case BspLumpType.LumpTexInfo:
                    Lumps[(int)lumpType] = ReadTexInfoLump(lump, binaryReader);
                    break;
                default:
                    Logging.Log($"No lump reader for {lumpType}", Logging.Severity.Medium);
                    break;
            }

            // seek back to original pos
            binaryReader.BaseStream.Seek(startOffset, SeekOrigin.Begin);
        }

        private Lump<TexInfo> ReadTexInfoLump(BSPLumpInfo lump, BinaryReader binaryReader)
        {
            var texInfoCount = lump.length / 4; // Edges are 4 bytes long (2 int16)
            var texInfoLump = new Lump<TexInfo>();
            Logging.Log($"Edge lump size: {texInfoCount}");

            for (int i = 0; i < texInfoCount; ++i)
            {
                // Read vertex
                texInfoLump.Contents.Add(new TexInfo()
                {
                    textureVecs = new float[2, 4]
                    {
                        { binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle() },
                        { binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle() },
                    },
                    lightmapVecs = new float[4, 2]
                    {
                        { binaryReader.ReadSingle(), binaryReader.ReadSingle() },
                        { binaryReader.ReadSingle(), binaryReader.ReadSingle() },
                        { binaryReader.ReadSingle(), binaryReader.ReadSingle() },
                        { binaryReader.ReadSingle(), binaryReader.ReadSingle() }
                    },
                    flags = binaryReader.ReadInt32(),
                    texData = binaryReader.ReadInt32()
                });
            }

            return texInfoLump;
        }

        private Lump<Edge> ReadEdgesLump(BSPLumpInfo lump, BinaryReader binaryReader)
        {
            var edgeCount = lump.length / 4; // Edges are 4 bytes long (2 int16)
            var edgeLump = new Lump<Edge>();
            Logging.Log($"Edge lump size: {edgeCount}");

            for (int i = 0; i < edgeCount; ++i)
            {
                // Read vertex
                edgeLump.Contents.Add(new Edge
                {
                    vertexIndices = new[] { binaryReader.ReadUInt16(), binaryReader.ReadUInt16() }
                });
            }

            return edgeLump;
        }

        private Lump<int> ReadSurfEdgesLump(BSPLumpInfo lump, BinaryReader binaryReader)
        {
            var surfEdgeCount = lump.length / 4; // Edges are 4 bytes long (1 int32)
            var surfEdgeLump = new Lump<int>();
            Logging.Log($"Surfedge lump size: {surfEdgeCount}");

            for (int i = 0; i < surfEdgeCount; ++i)
            {
                // Read vertex
                surfEdgeLump.Contents.Add(binaryReader.ReadInt32());
            }

            return surfEdgeLump;
        }

        private Lump<Plane> ReadPlaneLump(BSPLumpInfo lump, BinaryReader binaryReader)
        {
            var planeCount = lump.length / 20; // Planes are 20 bytes long
            var planeLump = new Lump<Plane>();
            Logging.Log($"Plane lump size: {planeCount}");

            for (int i = 0; i < planeCount; ++i)
            {
                // Read plane
                planeLump.Contents.Add(new Plane
                {
                    normal = binaryReader.ReadVector3(),
                    dist = binaryReader.ReadSingle(),
                    type = binaryReader.ReadInt32()
                });
            }

            return planeLump;
        }

        private Lump<Vector3> ReadVertexesLump(BSPLumpInfo lump, BinaryReader binaryReader)
        {
            var vertexCount = lump.length / 12; // Vertices are 12 bytes long (3 float)
            var vertexLump = new Lump<Vector3>();
            Logging.Log($"Vertex lump size: {vertexCount}");

            for (int i = 0; i < vertexCount; ++i)
            {
                // Read vertex
                vertexLump.Contents.Add(binaryReader.ReadVector3());
            }

            return vertexLump;
        }

        private Lump<Face> ReadFacesLump(BSPLumpInfo lump, BinaryReader binaryReader)
        {
            var faceCount = lump.length / 56; // Faces are 56 bytes long
            var faceLump = new Lump<Face>();
            Logging.Log($"Face lump size: {faceCount}");

            for (int i = 0; i < faceCount; ++i)
            {
                // Read face
                faceLump.Contents.Add(new Face
                {
                    planeNumber = binaryReader.ReadUInt16(),
                    side = binaryReader.ReadByte(),
                    onNode = binaryReader.ReadByte() == 1,
                    firstSurfEdge = binaryReader.ReadInt32(),
                    numSurfEdges = binaryReader.ReadInt16(),
                    texInfo = binaryReader.ReadInt16(),
                    dispInfo = binaryReader.ReadInt16(),
                    surfaceFogVolumeId = binaryReader.ReadInt16(),
                    styles = binaryReader.ReadBytes(4),
                    lightmapOffset = binaryReader.ReadInt32(),
                    area = binaryReader.ReadSingle(),
                    lightmapTextureMinsInLuxels = new[] {
                        binaryReader.ReadInt32(), binaryReader.ReadInt32()
                    },
                    lightmapTextureSizeInLuxels = new[] {
                        binaryReader.ReadInt32(), binaryReader.ReadInt32()
                    },
                    origFace = binaryReader.ReadInt32(),
                    numPrims = binaryReader.ReadUInt16(),
                    firstPrimId = binaryReader.ReadUInt16(),
                    smoothingGroups = binaryReader.ReadUInt32()
                });
            }

            return faceLump;
        }
    }
}

