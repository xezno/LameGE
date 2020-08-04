using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Render;
using Engine.Utils.MathUtils;
using Ulaid.Components;

namespace Ulaid.Entities
{
    public sealed class LevelModelEntity : Entity<LevelModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        public LevelModelEntity()
        {
            AddComponent(new TransformComponent(new Vector3d(0, 300f, 0f),
                                                new Vector3d(270, 0, 0),
                                                new Vector3d(1, 1, 1)/* * bspScaleFactor*/));
            AddComponent(new ShaderComponent(new Shader("Content/Shaders/Standard/standard.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/Standard/standard.vert", Shader.Type.VertexShader)));
            AddComponent(new BSPMeshComponent("Content/Maps/gm_flatgrass.bsp"));
            AddComponent(new MaterialComponent(new Material($"Content/Materials/level01.mtl")));
        }
    }
}