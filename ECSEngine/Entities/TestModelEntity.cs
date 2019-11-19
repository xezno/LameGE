using System.Collections;
using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Render;
using ECSEngine.Systems;

namespace ECSEngine.Entities
{
    public sealed class TestModelEntity : Entity<TestModelEntity>
    {
        private Material mainMaterial;
        public TestModelEntity()
        {
            // Add mesh component
            AddComponent(new ShaderComponent(new Shader("Content/main.frag", OpenGL.ShaderType.FragmentShader), new Shader("Content/main.vert", OpenGL.ShaderType.VertexShader)));
            AddMeshAndMaterialComponents("Content/PBRTest/MetalBall");
        }

        private void AddMeshAndMaterialComponents(string path)
        {
            mainMaterial = new Material($"{path}.mtl");
            AddComponent(new MaterialComponent(mainMaterial));
            AddComponent(new MeshComponent($"{path}.obj"));
        }

        public override void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            switch (eventType)
            {
                case Event.GameStart:
                    Program.game.GetSystem<ImGuiSystem>().AddSerializableObject(mainMaterial); // this is terrible
                    break;
            }
        }
    }
}
