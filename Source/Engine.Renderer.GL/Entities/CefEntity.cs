using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Render;
using Engine.Utils.MathUtils;
using System.IO;
using System.Reflection;

namespace Engine.Renderer.GL.Entities
{
    // TODO: ECS - Rename to HudEntity
    public sealed class CefEntity : Entity<CefEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.Wrench;

        public CefEntity()
        {
            AddComponent(new ShaderComponent(new Shader("Content/UI/Shaders/ui.frag", Shader.Type.FragmentShader),
                new Shader("Content/UI/Shaders/ui.vert", Shader.Type.VertexShader)));
            AddComponent(new TransformComponent(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
            AddComponent(new MaterialComponent(new Material("Content/UI/plane.mtl")));

            AddComponent(new MeshComponent(Primitives.Plane));
            AddComponent(new CefComponent($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Content/UI/index.html"));
        }
    }
}
