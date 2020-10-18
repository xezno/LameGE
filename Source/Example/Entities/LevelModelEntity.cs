using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Utils.MathUtils;
using Example.Components;
using Quincy.Components;

namespace Example.Entities
{
    public sealed class LevelModelEntity : Entity<LevelModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        public LevelModelEntity(string bspPath)
        {
            AddComponent(new TransformComponent(new Vector3d(0, 300f, 0f),
                                                new Vector3d(270, 0, 0),
                                                new Vector3d(1, 1, 1)));
            AddComponent(new ShaderComponent("Content/Shaders/standard.json"));
            AddComponent(new BSPMeshComponent(bspPath));
        }
    }
}