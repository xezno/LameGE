using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Utils;
using Engine.Utils.MathUtils;
using ExampleGame.Components;
using Engine.Renderer.Components;

namespace ExampleGame.Entities
{
    public class VoxelChunkEntity : Entity<VoxelChunkEntity>
    {
        public override string IconGlyph => FontAwesome5.Cubes;

        public VoxelChunkEntity(int seed, int xIndex, int zIndex)
        {
            AddComponent(new TransformComponent(new Vector3d(0, 0, 0),
                                                new Vector3d(0, 0, 0),
                                                new Vector3d(1, 1, 1)));
            AddComponent(new ShaderComponent(ServiceLocator.FileSystem.GetAsset("/Shaders/standard.json")));
            AddComponent(new VoxelChunkComponent(seed, xIndex, zIndex));
        }
    }
}
