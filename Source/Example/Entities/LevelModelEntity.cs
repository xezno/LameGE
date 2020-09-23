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

        public LevelModelEntity()
        {
            AddComponent(new TransformComponent(new Vector3d(0, 300f, 0f),
                                                new Vector3d(270, 0, 0),
                                                new Vector3d(1, 1, 1)/* * bspScaleFactor*/));
            AddComponent(new ShaderComponent("Content/Shaders/Standard/standard.frag", "Content/Shaders/Standard/standard.vert"));
            // AddComponent(new BSPMeshComponent("Content/Maps/gm_flatgrass.bsp"));
        }
    }
}