using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Render;
using Engine.Utils.MathUtils;

namespace Ulaid.Entities
{
    public sealed class TestCubeEntity : Entity<LevelModelEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.LayerGroup;

        private Material mainMaterial;

        public TestCubeEntity()
        {
            AddComponent(new TransformComponent(new Vector3d(0, 0, 0f),
                                                new Vector3d(270, 0, 0),
                                                new Vector3d(1, 1, 1)));

            AddComponent(new ShaderComponent(new Shader("Content/Shaders/Standard/standard.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/Standard/standard.vert", Shader.Type.VertexShader)));
            AddComponent(new MaterialComponent(new Material($"Content/Materials/level01.mtl")));
            AddComponent(new MeshComponent($"Content/Models/cube.obj"));
        }
    }
}