using Engine.ECS.Components;
using Engine.ECS.Entities;
using Engine.ECS.Observer;
using Engine.Utils;
using Engine.Utils.DebugUtils;
using Engine.Utils.MathUtils;
using ImGuiNET;
using OpenGL;
using Engine.Renderer;
using Engine.Renderer.Components;
using Engine.Renderer.Primitives;
using System;
using System.Collections.Generic;

namespace ExampleGame.Components
{
    public class VoxelChunkComponent : Component<BSPMeshComponent>
    {
        public struct Voxel
        {
            public enum VoxelId
            {
                Air,
                Filled
            }

            public VoxelId Id { get; set; }
        }
        public struct TerrainOctave
        {
            public float intensity;
            public float scale;
        }

        public /*const*/ int chunkSize = 16;
        public /*const*/ int chunkHeight = 256;
        public int seed;
        public TerrainOctave[] octaves = new[] { new TerrainOctave() { intensity = 1.0f, scale = 8.0f } };
        public FastNoise noise;
        public float threshold = 0.2f;

        private int xPos;
        private int zPos;

        public VoxelChunkComponent(int seed, int xIndex, int zIndex) 
        {
            this.seed = seed;
            xPos = xIndex * chunkSize;
            zPos = zIndex * chunkSize;
            // zPos = zIndex * chunkHeight;
            noise = new FastNoise(seed);
        }

        private float GetNoise(float x, float y, float z, float noiseScale, float noiseIntensity)
        {
            return (noise.GetPerlin(x * noiseScale, y * noiseScale, z * noiseScale) * noiseIntensity + 0.5f) / 2f;
        }

        private Voxel[,,] GenerateVoxelMap()
        {
            Voxel[,,] voxels = new Voxel[chunkSize, chunkHeight, chunkSize];

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        float value = 0f;
                        foreach (var octave in octaves)
                        {
                            value += GetNoise(x + xPos, y, z + zPos, octave.scale, octave.intensity);
                        }
                        if (value > threshold)
                        {
                            voxels[x, y, z].Id = Voxel.VoxelId.Filled;
                        }
                        else
                        {
                            voxels[x, y, z].Id = Voxel.VoxelId.Air;
                        }
                    }
                }
            }

            return voxels;
        }

        private void GenerateMesh()
        {
            // Generate cube
            var cube = new Cube();
            var modelComponent = new ModelComponent();
            var textures = new List<Texture>()
            {
                Texture.LoadFromAsset(ServiceLocator.FileSystem.GetAsset("Textures/holoMap.png"), "texture_diffuse")
            };

            modelComponent.Meshes = new List<Mesh>();

            List<Vertex> vertices = new List<Vertex>();
            var voxels = GenerateVoxelMap();

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        // Check if neighbouring voxel is filled or not
                        if (voxels[x, y, z].Id != Voxel.VoxelId.Air)
                        {
                            // Voxel isn't filled - check surroundings, fill vertices appropriately
                            // TODO: Programatically determine vertices instead of just adding cubes

                            var addVertices = false;
                            if (x + 1 == chunkSize || voxels[x + 1, y, z].Id == Voxel.VoxelId.Air)
                            {
                                addVertices = true;
                            }
                            else if (x - 1 < 0 || voxels[x - 1, y, z].Id == Voxel.VoxelId.Air)
                            {
                                addVertices = true;
                            }

                            if (y + 1 == chunkHeight || voxels[x, y + 1, z].Id == Voxel.VoxelId.Air)
                            {
                                addVertices = true;
                            }
                            else if (y - 1 < 0 || voxels[x, y - 1, z].Id == Voxel.VoxelId.Air)
                            {
                                addVertices = true;
                            }

                            if (z + 1 == chunkSize || voxels[x, y, z + 1].Id == Voxel.VoxelId.Air)
                            {
                                addVertices = true;
                            }
                            else if (z - 1 < 0 || voxels[x, y, z - 1].Id == Voxel.VoxelId.Air)
                            {
                                addVertices = true;
                            }

                            if (addVertices)
                            {
                                for (int i = 0; i < cube.Vertices.Count; i++)
                                {
                                    var vertex = cube.Vertices[i];
                                    vertex.Position += new Vector3f(x, y, z) * 2f;
                                    vertices.Add(vertex);
                                }
                            }
                        }
                    }
                }
            }

            var indices = new List<uint>();
            for (uint i = 0; i < vertices.Count; ++i)
                indices.Add(i);

            Logging.Log($"Generated ({xPos / chunkSize}, {zPos / chunkSize})");
            modelComponent.Meshes.Add(new Mesh(vertices, indices, textures, Matrix4x4f.Identity));

            // TODO: Derive from MeshComponent instead
            var parent = (IEntity)Parent;
            if (parent.HasComponent<ModelComponent>())
                parent.RemoveComponent<ModelComponent>();
            parent.AddComponent(modelComponent);
        }

        public override void RenderImGui()
        {
            if (ImGui.Button("Generate Terrain"))
            {
                GenerateMesh();
            }
            base.RenderImGui();
        }

        public override void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
            base.OnNotify(notifyType, notifyArgs);
            switch (notifyType)
            {
                case NotifyType.ContextReady:
                    GenerateMesh();
                    break;
            }
        }
    }
}
