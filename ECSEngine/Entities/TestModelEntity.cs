using System.Collections;
using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.Math;
using ECSEngine.Render;
using ECSEngine.Systems;

namespace ECSEngine.Entities
{
    public sealed class TestModelEntity : Entity<TestModelEntity>
    {
        private Material mainMaterial;
        private TransformComponent transformComponent;
        public TestModelEntity()
        {
            // Add mesh component
            transformComponent = new TransformComponent(new Vector3(0, 0, 5f), Quaternion.identity, new Vector3(1, 1, 1));
            AddComponent(transformComponent); 
            AddComponent(new ShaderComponent(new Shader("Content/main.frag", OpenGL.ShaderType.FragmentShader), new Shader("Content/main.vert", OpenGL.ShaderType.VertexShader)));
            AddMeshAndMaterialComponents("Content/level01");
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
                    Program.game.GetSystem<ImGuiSystem>().AddSerializableObject(transformComponent); // this is also terrible
                    break;
            }
        }
    }
}
