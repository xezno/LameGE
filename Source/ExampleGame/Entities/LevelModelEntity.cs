using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Utils;
using Engine.Utils.FileUtils;
using Engine.Utils.MathUtils;
using ExampleGame.Components;
using Engine.Renderer.Components;

namespace ExampleGame.Entities
{
    public sealed class LevelModelEntity : Entity<LevelModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        public LevelModelEntity(Asset asset)
        {
            AddComponent(new TransformComponent(new Vector3d(0, 300f, 0f),
                                                new Vector3d(270, 0, 0),
                                                new Vector3d(1, 1, 1)));
            AddComponent(new ShaderComponent(ServiceLocator.FileSystem.GetAsset("/Shaders/standard.json")));
            AddComponent(new BSPMeshComponent(asset));
        }
    }
}