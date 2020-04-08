using ECSEngine.Components;
using ECSEngine.Entities;
using ECSEngine.MathUtils;
using ECSEngine.Render;
using OpenGL;
using Quaternion = ECSEngine.MathUtils.Quaternion;

namespace UlaidGame.Entities
{
    public sealed class PlanetEntity : Entity<PlanetEntity>
    {
        private Material mainMaterial;
        public PlanetEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, -2f), Quaternion.identity, new Vector3(1, 1, 1)));
            AddComponent(new ShaderComponent(new Shader("Content/Planet/main.frag", ShaderType.FragmentShader), new Shader("Content/Planet/main.vert", ShaderType.VertexShader)));
            AddMeshAndMaterialComponents("Content/Planet/PlanetTest");
        }

        private void AddMeshAndMaterialComponents(string path)
        {
            mainMaterial = new Material($"{path}.mtl");
            AddComponent(new MaterialComponent(mainMaterial));
            AddComponent(new MeshComponent($"{path}.obj"));
        }
    }
}