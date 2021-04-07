using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Renderer.Components;
using Engine.Utils;
using Engine.Utils.FileUtils;
using Engine.Utils.MathUtils;
using System.Numerics;

namespace ExampleGame.Entities
{
    public sealed class ModelEntity : Entity<ModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        public ModelEntity(Asset modelAsset, Vector3 position, Vector3 scale)
        {
            AddComponent(new TransformComponent(position,
                                                new Vector3(0, 0, 0),
                                                scale));

            AddComponent(new ShaderComponent(ServiceLocator.FileSystem.GetAsset("/Shaders/pbr.json")));
            AddComponent(new ModelComponent(modelAsset));
        }
    }
}