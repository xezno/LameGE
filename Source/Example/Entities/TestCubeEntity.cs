using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Utils.MathUtils;
using Quincy.Components;

namespace Example.Entities
{
    public sealed class TestCubeEntity : Entity<LevelModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        public TestCubeEntity()
        {
            AddComponent(new TransformComponent(new Vector3d(0, 0, 0f),
                                                new Vector3d(270, 0, 0),
                                                new Vector3d(1, 1, 1)));

            AddComponent(new ShaderComponent("Content/Shaders/Standard/standard.frag", "Content/Shaders/Standard/standard.vert"));
            AddComponent(new ModelComponent($"Content/Models/cube.obj"));
        }
    }
}