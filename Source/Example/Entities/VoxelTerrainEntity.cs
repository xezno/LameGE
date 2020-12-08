using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Utils;
using Engine.Utils.MathUtils;
using Example.Components;
using Quincy.Components;

namespace Example.Entities
{
    public class VoxelTerrainEntity : Entity<VoxelTerrainEntity>
    {
        public override string IconGlyph => FontAwesome5.Cubes;

        public VoxelTerrainEntity()
        {
            AddComponent(new TransformComponent(new Vector3d(0, 0, 0),
                                                new Vector3d(0, 0, 0),
                                                new Vector3d(1, 1, 1)));
            AddComponent(new ShaderComponent(ServiceLocator.FileSystem.GetAsset("/Shaders/standard.json")));
            AddComponent(new VoxelTerrainComponent());
        }
    }
}
