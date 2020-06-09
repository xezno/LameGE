using BepuPhysics.Collidables;
using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Managers;
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
            AddComponent(new TransformComponent(new Vector3(0, 0, 0f),
                                                new Vector3(270, 0, 0),
                                                new Vector3(1, 1, 1)));

            AddComponent(new ShaderComponent(new Shader("Content/Shaders/Standard/main.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/Standard/main.vert", Shader.Type.VertexShader)));

            AddMeshAndMaterialComponents("level01");
        }

        public override void Update(float deltaTime) { }

        private void AddMeshAndMaterialComponents(string path)
        {
            mainMaterial = new Material($"Content/Materials/{path}.mtl");
            AddComponent(new MaterialComponent(mainMaterial));
            AddComponent(new MeshComponent($"Content/Models/cube.obj"));
        }
    }
}