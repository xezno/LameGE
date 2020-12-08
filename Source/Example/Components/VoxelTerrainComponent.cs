using Engine.ECS.Components;
using Engine.ECS.Entities;
using Engine.ECS.Observer;
using Engine.Utils;
using OpenGL;
using Quincy;
using Quincy.Components;
using Quincy.Primitives;
using System.Collections.Generic;

namespace Example.Components
{
    public class VoxelTerrainComponent : Component<BSPMeshComponent>
    {
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

            const int chunkSize = 16;
            modelComponent.Meshes = new List<Mesh>();
            for (int x = 0; x < chunkSize; ++x)
            {
                for (int y = 0; y < chunkSize; ++y)
                {
                    for (int z = 0; z < chunkSize; ++z)
                    {
                        var modelMat = Matrix4x4f.Identity;
                        modelMat.Translate(x, y, z);
                        modelMat.Scale(.5f, .5f, .5f);
                        var mesh = new Mesh(cube.Vertices, cube.Indices, textures, modelMat);
                        modelComponent.Meshes.Add(mesh);
                    }
                }
            }

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
