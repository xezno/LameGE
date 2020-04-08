using System;
using ECSEngine.Components;
using ECSEngine.Entities;
using ECSEngine.MathUtils;
using ECSEngine.Render;
using OpenGL;
using UlaidGame.Assets.BSP;
using UlaidGame.Assets.BSP.Lumps;
using UlaidGame.Assets.BSP.Types;

namespace UlaidGame.Entities
{
    public sealed class LevelModelEntity : Entity<LevelModelEntity>
    {
        private BSPLoader bspLoader;
        private Material mainMaterial;
        public LevelModelEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, -2f),
                                                new Vector3(270, 0, 0),
                                                new Vector3(1, 1, 1) * 0.01f));
            AddComponent(new ShaderComponent(new Shader("Content/main.frag", ShaderType.FragmentShader),
                new Shader("Content/main.vert", ShaderType.VertexShader)));

            bspLoader = new BSPLoader("Content/de_MW2_Terminal_v1.bsp");
            AddMeshAndMaterialComponents("Content/level01");
        }

        private void GenerateBSPMesh()
        {
            var meshComponent = new MeshComponent();
            var vertexLump = (VertexLump)bspLoader.Lumps[(int) BspLumpType.LumpVertexes];
            var planeLump = (PlaneLump)bspLoader.Lumps[(int)BspLumpType.LumpPlanes];
            var edgeLump = (EdgeLump)bspLoader.Lumps[(int)BspLumpType.LumpEdges];
            var surfEdgeLump = (SurfEdgeLump)bspLoader.Lumps[(int)BspLumpType.LumpSurfEdges];
            var faceLump = (FaceLump) bspLoader.Lumps[(int) BspLumpType.LumpFaces];

            // setup vertices
            foreach (var vertex in vertexLump.Contents)
            {
                meshComponent.RenderMesh.vertices.Add(vertex);
                meshComponent.RenderMesh.uvCoords.Add(new Vector2(0, 0));
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
                        else
                            firstPoint = vertPoint;

                        vertPoint = edge.vertexIndices[reversed ? 1 : 0];

                        if (vertPoint == rootPoint)
                            continue;
                        else
                            secondPoint = vertPoint;


                        meshComponent.RenderMesh.faceElements.Add(new MeshFaceElement(
                            (uint)rootPoint,
                            0,
                            face.planeNumber
                        ));
                        meshComponent.RenderMesh.faceElements.Add(new MeshFaceElement(
                            (uint)firstPoint,
                            0,
                            face.planeNumber
                        ));
                        meshComponent.RenderMesh.faceElements.Add(new MeshFaceElement(
                            (uint)secondPoint,
                            0,
                            face.planeNumber
                        ));
                    }
                }
            }

            meshComponent.RenderMesh.GenerateBuffers();
            AddComponent(meshComponent);
        }
        private void AddMeshAndMaterialComponents(string path)
        {
            mainMaterial = new Material($"{path}.mtl");
            AddComponent(new MaterialComponent(mainMaterial));
            GenerateBSPMesh();
        }
    }
}