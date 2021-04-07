using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Renderer.Components;
using Engine.Utils;
using Engine.Utils.MathUtils;
using System.Numerics;

namespace Engine.Renderer.Entities
{
    public sealed class LightEntity : Entity<LightEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.Lightbulb;

        public LightEntity()
        {
            var fs = ServiceLocator.FileSystem;
            AddComponent(new ShaderComponent(fs.GetAsset("/Shaders/PBR/pbr.frag"), fs.GetAsset("/Shaders/PBR/pbr.vert")));
            AddComponent(new TransformComponent(new Vector3(0, 5f, 0f), new Vector3(90, 0, 0), new Vector3(1, 1, 1)));
            AddComponent(new LightComponent());
            // AddComponent(new ModelComponent("Content/Models/arrow/scene.gltf"));
        }
    }
}