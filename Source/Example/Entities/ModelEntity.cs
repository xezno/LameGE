using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Utils.MathUtils;
using Quincy.Components;

namespace Example.Entities
{
    public sealed class ModelEntity : Entity<ModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        public ModelEntity(string modelPath, Vector3d scale)
        {
            AddComponent(new TransformComponent(new Vector3d(0, 0, 0f),
                                                new Vector3d(0, 0, 0),
                                                scale));

            AddComponent(new ShaderComponent("Content/Shaders/Standard/standard.frag", "Content/Shaders/Standard/standard.vert"));
            AddComponent(new ModelComponent(modelPath));
        }
    }
}