using System;
using System.IO;
using System.Linq;
using ECSEngine;
using UlaidGame.Assets.BSP.Lumps;
using UlaidGame.Assets.BSP.Types;

namespace UlaidGame.Assets.BSP
{
    class BSPLoader
    {
        private BSPHeader header;
        public BaseLump[] Lumps { get; } = new BaseLump[64];

        public BSPLoader(string path)
        {
            using var streamReader = new StreamReader(path);
            using var binaryReader = new BinaryReader(streamReader.BaseStream);
            Debug.Log($"Reading BSP file {path}");

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
                Debug.Log("Not a valid BSP file!", Debug.DebugSeverity.High);

            if (header.version != 20)
                Debug.Log($"BSP version {header.version} is unsupported; will try to parse map anyway.", Debug.DebugSeverity.Medium);

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
            Debug.Log($"BSP file has map revision {header.mapRevision}");
        }

        private void ReadLumps(BinaryReader binaryReader)
        {
            for (int i = 0; i < 64; ++i)
            {
                var lump = header.lumpDirectory[i];
                var lumpType = (BspLumpType)i;
                Debug.Log($"{lumpType} at {lump.offset} {lump.length}");
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

            //// read contents into buffer
            //byte[] lumpData = binaryReader.ReadBytes(lump.length);

            // handle lump based on lumptype given
            switch (lumpType)
            {
                case BspLumpType.LumpPlanes:
                    var planeCount = lump.length / 20; // Planes are 20 bytes long
                    var planeLump = new PlaneLump();
                    Debug.Log($"Plane lump size: {planeCount}");

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

                    Lumps[(int)lumpType] = planeLump;
                    Debug.Log("Read planes");

                    if (binaryReader.BaseStream.Position != lump.offset + lump.length)
                    {
                        throw new Exception("ah fuck");
                    }
                    break;
                case BspLumpType.LumpVertexes:
                    var vertexCount = lump.length / 12; // Vertices are 12 bytes long (3 float)
                    var vertexLump = new VertexLump();
                    Debug.Log($"Vertex lump size: {vertexCount}");

                    for (int i = 0; i < vertexCount; ++i)
                    {
                        // Read vertex
                        vertexLump.Contents.Add(binaryReader.ReadVector3());
                    }

                    Lumps[(int)lumpType] = vertexLump;
                    Debug.Log("Read vertices");

                    if (binaryReader.BaseStream.Position != lump.offset + lump.length)
                    {
                        throw new Exception("ah fuck");
                    }
                    break;
                case BspLumpType.LumpEdges:
                    var edgeCount = lump.length / 4; // Edges are 4 bytes long (2 int16)
                    var edgeLump = new EdgeLump();
                    Debug.Log($"Edge lump size: {edgeCount}");

                    for (int i = 0; i < edgeCount; ++i)
                    {
                        // Read vertex
                        edgeLump.Contents.Add(new Edge
                        {
                            vertexIndices = new [] { binaryReader.ReadUInt16(), binaryReader.ReadUInt16() }
                        });
                    }

                    Lumps[(int)lumpType] = edgeLump;
                    Debug.Log("Read edges");

                    if (binaryReader.BaseStream.Position != lump.offset + lump.length)
                    {
                        throw new Exception("ah fuck");
                    }
                    break; ;
                case BspLumpType.LumpSurfEdges:
                    var surfEdgeCount = lump.length / 4; // Edges are 4 bytes long (1 int32)
                    var surfEdgeLump = new SurfEdgeLump();
                    Debug.Log($"Surfedge lump size: {surfEdgeCount}");

                    for (int i = 0; i < surfEdgeCount; ++i)
                    {
                        // Read vertex
                        surfEdgeLump.Contents.Add(binaryReader.ReadInt32());
                    }

                    Lumps[(int)lumpType] = surfEdgeLump;
                    Debug.Log("Read surfedges");

                    if (binaryReader.BaseStream.Position != lump.offset + lump.length)
                    {
                        throw new Exception("ah fuck");
                    }
                    break;
                case BspLumpType.LumpFaces:
                    var faceCount = lump.length / 56; // Faces are 56 bytes long
                    var faceLump = new FaceLump();
                    Debug.Log($"Face lump size: {faceCount}");

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
                            lightmapTextureMinsInLuxels = new[] { binaryReader.ReadInt32(), binaryReader.ReadInt32() },
                            lightmapTextureSizeInLuxels = new[] { binaryReader.ReadInt32(), binaryReader.ReadInt32() },
                            origFace = binaryReader.ReadInt32(),
                            numPrims = binaryReader.ReadUInt16(),
                            firstPrimId = binaryReader.ReadUInt16(),
                            smoothingGroups = binaryReader.ReadUInt32()
                        });
                    }

                    Lumps[(int)lumpType] = faceLump;
                    Debug.Log("Read faces");

                    if (binaryReader.BaseStream.Position != lump.offset + lump.length)
                    {
                        throw new Exception("ah fuck");
                    }
                    break;
                default:
                    Debug.Log($"No reader for lump {lumpType}", Debug.DebugSeverity.Medium);
                    break;
            }
            
            // seek back to original pos
            binaryReader.BaseStream.Seek(startOffset, SeekOrigin.Begin);
        }
    }
}

