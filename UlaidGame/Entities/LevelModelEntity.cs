using BepuPhysics;
using BepuPhysics.Collidables;
using ECSEngine.Assets;
using ECSEngine.Components;
using ECSEngine.DebugUtils;
using ECSEngine.Entities;
using ECSEngine.Managers;
using ECSEngine.MathUtils;
using ECSEngine.Render;
using OpenGL;
using System;
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
        private BepuPhysics.Collidables.Mesh physicsMesh;
        private int physicsIndex;
        private float bspScaleFactor = 0.0254000508f;

        public LevelModelEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 300f, 0f),
                                                new Vector3(270, 0, 0),
                                                new Vector3(1, 1, 1) * bspScaleFactor));
            AddComponent(new ShaderComponent(new Shader("Content/Shaders/Standard/main.frag", ShaderType.FragmentShader),
                new Shader("Content/Shaders/Standard/main.vert", ShaderType.VertexShader)));

            bspLoader = new BSPLoader("Content/Maps/gm_flatgrass.bsp");
            AddMeshAndMaterialComponents("level01");
            
            var degToRad = 0.0174533f;
            var transform = GetComponent<TransformComponent>();
            // Add physics
            physicsIndex = PhysicsManager.Instance.Simulation.Statics.Add(
                new StaticDescription(transform.Position.ConvertToNumerics(), 
                BepuUtilities.Quaternion.CreateFromYawPitchRoll(transform.RotationEuler.x * degToRad, transform.RotationEuler.y * degToRad, transform.RotationEuler.z * degToRad), 
                new CollidableDescription(PhysicsManager.Instance.Simulation.Shapes.Add(physicsMesh), 0.1f)
            ));
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

            int triCount = 0;
            foreach (var face in faceLump.Contents)
            {
                int rootPoint = 0, firstPoint = 0, secondPoint = 0;
                for (int surfEdgeNum = 0; surfEdgeNum < face.numSurfEdges; surfEdgeNum++)
                {
                    var surfEdgeIndex = face.firstSurfEdge + surfEdgeNum;
                    var edgeIndex = surfEdgeLump.Contents[surfEdgeIndex];

                    bool reversed = edgeIndex < 0;
                    Edge edge = edgeLump.Contents[Math.Abs(edgeIndex)];

                    int vertPoint;
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
                    
                    triCount++;
                }
            }
            Logging.Log($"BSP has {triCount} tris");
            var triangleIndex = 0;

            PhysicsManager.Instance.BufferPool.Take<Triangle>(triCount, out var triangles);

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
            }

            // setup normals
            foreach (var plane in planeLump.Contents)
            {
                meshComponent.RenderMesh.normals.Add(plane.normal);
            }

            foreach (var face in faceLump.Contents)
            {
                if (!face.onNode) continue;
                int rootPoint = 0, firstPoint = 0, secondPoint = 0;
                for (int surfEdgeNum = 0; surfEdgeNum < face.numSurfEdges; surfEdgeNum++)
                {
                    var surfEdgeIndex = face.firstSurfEdge + surfEdgeNum;
                    var edgeIndex = surfEdgeLump.Contents[surfEdgeIndex];

                    // big thanks to https://github.com/ajkhoury/OpenBSP-MinGW for this bit - the vbsp docs aren't great

                    bool reversed = edgeIndex < 0;
                    Edge edge = edgeLump.Contents[Math.Abs(edgeIndex)];

                    int vertPoint;
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

                    var texInfo = texInfoLump.Contents[face.texInfo];
                    // u = tv(0,0) * x + tv(0,1) * y + tv(0,2) * z + tv(0,3)
                    // v = tv(1,0) * x + tv(1,1) * y + tv(1,2) * z + tv(1,3)

                    var rootPointCoords = vertexLump.Contents[rootPoint];
                    var firstPointCoords = vertexLump.Contents[firstPoint];
                    var secondPointCoords = vertexLump.Contents[secondPoint];

                    meshComponent.RenderMesh.uvCoords.Add(GetUVCoords(texInfo, rootPointCoords));
                    meshComponent.RenderMesh.uvCoords.Add(GetUVCoords(texInfo, firstPointCoords));
                    meshComponent.RenderMesh.uvCoords.Add(GetUVCoords(texInfo, secondPointCoords));

                    meshComponent.RenderMesh.faceElements.Add(new MeshFaceElement(
                        (uint)rootPoint,
                        (uint)meshComponent.RenderMesh.uvCoords.Count - 3,
                        face.planeNumber
                    ));
                    meshComponent.RenderMesh.faceElements.Add(new MeshFaceElement(
                        (uint)firstPoint,
                        (uint)meshComponent.RenderMesh.uvCoords.Count - 2,
                        face.planeNumber
                    ));
                    meshComponent.RenderMesh.faceElements.Add(new MeshFaceElement(
                        (uint)secondPoint,
                        (uint)meshComponent.RenderMesh.uvCoords.Count - 1,
                        face.planeNumber
                    ));
                    
                    // Triangle:
                    // rootPoint - firstPoint - secondPoint
                    triangles[triangleIndex++] = new Triangle(
                        rootPointCoords.ConvertToNumerics(),
                        firstPointCoords.ConvertToNumerics(),
                        secondPointCoords.ConvertToNumerics()
                    );
                }
            }

            physicsMesh = new BepuPhysics.Collidables.Mesh(triangles, new System.Numerics.Vector3(1, 1, 1) * bspScaleFactor, PhysicsManager.Instance.BufferPool);

            meshComponent.RenderMesh.GenerateBuffers();
            AddComponent(meshComponent);
        }

        private Vector2 GetUVCoords(TexInfo texInfo, Vector3 coords)
        {
            var uCoord = texInfo.textureVecs[0, 0] * coords.x + texInfo.textureVecs[0, 1] * coords.y + texInfo.textureVecs[0, 2] * coords.z +
                             texInfo.textureVecs[0, 3];
            var vCoord = texInfo.textureVecs[1, 0] * coords.x + texInfo.textureVecs[1, 1] * coords.y + texInfo.textureVecs[1, 2] * coords.z +
                             texInfo.textureVecs[1, 3];

            vCoord = 1.0f - vCoord; // Flip for opengl

            return new Vector2(uCoord, vCoord) / 1000.0f;
        }

        private void AddMeshAndMaterialComponents(string path)
        {
            mainMaterial = new Material($"Content/Materials/{path}.mtl");
            AddComponent(new MaterialComponent(mainMaterial));
            GenerateBSPMesh();
        }
    }
}