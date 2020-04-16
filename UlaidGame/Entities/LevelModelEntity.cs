using ECSEngine.Components;
using ECSEngine.Entities;
using ECSEngine.MathUtils;
using ECSEngine.Render;
using OpenGL;
using System;
using ECSEngine.Assets;
using UlaidGame.Assets.BSP;
using UlaidGame.Assets.BSP.Lumps;
using UlaidGame.Assets.BSP.Types;

namespace UlaidGame.Entities
{
    public sealed class LevelModelEntity : Entity<LevelModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        private BSPLoader bspLoader;
        private Material mainMaterial;

        public LevelModelEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, -2f),
                                                new Vector3(270, 0, 0),
                                                new Vector3(1, 1, 1) * 0.01f));
            AddComponent(new ShaderComponent(new Shader("Content/Shaders/Standard/main.frag", ShaderType.FragmentShader),
                new Shader("Content/Shaders/Standard/main.vert", ShaderType.VertexShader)));

            bspLoader = new BSPLoader("Content/Maps/gm_flatgrass.bsp");
            AddMeshAndMaterialComponents("level01");
        }

        private void GenerateBSPMesh()
        {
            var meshComponent = new MeshComponent();
            var vertexLump = (Lump<Vector3>)bspLoader.Lumps[(int)BspLumpType.LumpVertexes];
            var planeLump = (Lump<Plane>)bspLoader.Lumps[(int)BspLumpType.LumpPlanes];
            var edgeLump = (Lump<Edge>)bspLoader.Lumps[(int)BspLumpType.LumpEdges];
            var surfEdgeLump = (Lump<int>)bspLoader.Lumps[(int)BspLumpType.LumpSurfEdges];
            var faceLump = (Lump<Face>)bspLoader.Lumps[(int)BspLumpType.LumpFaces];
            var texInfoLump = (Lump<TexInfo>)bspLoader.Lumps[(int)BspLumpType.LumpTexInfo];

            // setup vertices
            foreach (var vertex in vertexLump.Contents)
            {
                meshComponent.RenderMesh.vertices.Add(vertex);
            }

            // setup uv coords

            foreach (var texInfo in texInfoLump.Contents)
            {
                string logStr = "";
                for (int x = 0; x < 2; ++x)
                {
                    for (int y = 0; y < 4; ++y)
                    {
                        logStr += $"{texInfo.textureVecs[x, y]}\t";
                    }

                    logStr += "\n\t";
                }
                // u = tv(0,0) * x + tv(0,1) * y + tv(0,2) * z + tv(0,3)
                // v = tv(1,0) * x + tv(1,1) * y + tv(1,2) * z + tv(1,3)

                meshComponent.RenderMesh.uvCoords.Add(new Vector2(texInfo.textureVecs[0, 0], texInfo.textureVecs[0, 1]) / 1000f);
            }

            // setup normals
            foreach (var plane in planeLump.Contents)
            {
                meshComponent.RenderMesh.normals.Add(plane.normal);
            }

            foreach (var face in faceLump.Contents)
            {
                if (!face.onNode) continue;

                int rootPoint = 0, vertPoint = 0, firstPoint = 0, secondPoint = 0;
                for (int surfEdgeNum = 0; surfEdgeNum < face.numSurfEdges; surfEdgeNum++)
                {
                    var surfEdgeIndex = face.firstSurfEdge + surfEdgeNum;
                    var edgeIndex = surfEdgeLump.Contents[surfEdgeIndex];

                    // big thanks to https://github.com/ajkhoury/OpenBSP-MinGW for this bit - the vbsp docs aren't great

                    bool reversed = edgeIndex < 0;
                    Edge edge = edgeLump.Contents[Math.Abs(edgeIndex)];

                    if (surfEdgeNum == 0)
                    {
                        rootPoint = edge.vertexIndices[reversed ? 0 : 1];
                        vertPoint = edge.vertexIndices[reversed ? 1 : 0];
                    }
                    else
                    {
                        vertPoint = edge.vertexIndices[reversed ? 0 : 1];

                        if (vertPoint == rootPoint)
                            continue;
                        firstPoint = vertPoint;

                        vertPoint = edge.vertexIndices[reversed ? 1 : 0];

                        if (vertPoint == rootPoint)
                            continue;
                        secondPoint = vertPoint;
                    }

                    meshComponent.RenderMesh.faceElements.Add(new MeshFaceElement(
                        (uint)rootPoint,
                        (uint)face.texInfo,
                        face.planeNumber
                    ));
                    meshComponent.RenderMesh.faceElements.Add(new MeshFaceElement(
                        (uint)firstPoint,
                        (uint)face.texInfo,
                        face.planeNumber
                    ));
                    meshComponent.RenderMesh.faceElements.Add(new MeshFaceElement(
                        (uint)secondPoint,
                        (uint)face.texInfo,
                        face.planeNumber
                    ));
                }
            }

            meshComponent.RenderMesh.GenerateBuffers();
            AddComponent(meshComponent);
        }
        private void AddMeshAndMaterialComponents(string path)
        {
            mainMaterial = new Material($"Content/Materials/{path}.mtl");
            AddComponent(new MaterialComponent(mainMaterial));
            GenerateBSPMesh();
        }
    }
}