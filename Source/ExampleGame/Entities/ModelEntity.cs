using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Renderer.Components;
using Engine.Utils;
using Engine.Utils.FileUtils;
using Engine.Utils.MathUtils;

namespace ExampleGame.Entities
{
    public sealed class ModelEntity : Entity<ModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        public ModelEntity(Asset modelAsset, Vector3d position, Vector3d scale)
        {
            AddComponent(new TransformComponent(position,
                                                new Vector3d(0, 0, 0),
                                                scale));

            AddComponent(new ShaderComponent(ServiceLocator.FileSystem.GetAsset("/Shaders/pbr.json")));
            AddComponent(new ModelComponent(modelAsset));
        }
    }
}