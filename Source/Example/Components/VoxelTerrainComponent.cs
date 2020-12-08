using Engine.ECS.Components;
using Engine.ECS.Entities;
using Engine.ECS.Observer;
using Engine.Utils;
using Engine.Utils.MathUtils;
using OpenGL;
using Quincy;
using Quincy.Components;
using Quincy.Primitives;
using System.Collections.Generic;

namespace Example.Components
{
    public class VoxelTerrainComponent : Component<BSPMeshComponent>
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

        public VoxelTerrainComponent() { }

        private void GenerateMesh()
        {
            // Generate cube
            var cube = new Cube();
            var modelComponent = new ModelComponent();
            var textures = new List<Texture>()
            {
                Texture.LoadFromAsset(ServiceLocator.FileSystem.GetAsset("Textures/holoMap.png"), "texture_diffuse")
            };

            const int chunkSize = 8;
            const int chunkHeight = 256;

            modelComponent.Meshes = new List<Mesh>();

            List<Vertex> vertices = new List<Vertex>();
            Voxel[,,] voxels = new Voxel[chunkSize, chunkHeight, chunkSize];

            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        voxels[x, y, z] = new Voxel() { Id = Voxel.VoxelId.Filled };
                    }
                }
            }

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

            //for (int x = 0; x < chunkSize; ++x)
            //{
            //    for (int y = 0; y < chunkSize; ++y)
            //    {
            //        for (int z = 0; z < chunkSize; ++z)
            //        {
            //            var modelMat = Matrix4x4f.Identity;
            //            modelMat.Translate(x, y, z);
            //            modelMat.Scale(.5f, .5f, .5f);


            //            var mesh = new Mesh(cube.Vertices, cube.Indices, textures, modelMat);
            //            modelComponent.Meshes.Add(mesh);
            //        }
            //    }
            //}

            var indices = new List<uint>();
            for (uint i = 0; i < vertices.Count; ++i)
                indices.Add(i);

            modelComponent.Meshes.Add(new Mesh(vertices, indices, textures, Matrix4x4f.Identity));

            // TODO: Derive from MeshComponent instead
            ((IEntity)Parent).AddComponent(modelComponent);
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
