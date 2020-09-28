using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Utils.MathUtils;
using Quincy.Components;

namespace Example.Entities
{
    public sealed class ModelEntity : Entity<ModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        public ModelEntity(string modelPath, Vector3d position, Vector3d scale)
        {
            AddComponent(new TransformComponent(position,
                                                new Vector3d(0, 0, 0),
                                                scale));

            AddComponent(new ShaderComponent("Content/Shaders/PBR/pbr.frag", "Content/Shaders/PBR/pbr.vert"));
            AddComponent(new ModelComponent(modelPath));
        }
    }
}