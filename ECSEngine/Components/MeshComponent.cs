using ECSEngine.Attributes;
using ECSEngine.Events;
using ECSEngine.Render;

using OpenGL;

namespace ECSEngine.Components
{
    /// <summary>
    /// Contains the component-based code used in order to render
    /// a mesh on-screen.
    /// </summary>
    [Requires(typeof(ShaderComponent))]
    public class MeshComponent : Component<MeshComponent>
    {
        Mesh mesh;
        public MeshComponent(string path)
        {
            mesh = new Mesh(path);
        }

        public override void Render()
        {
            ((IEntity)parent).GetComponent<ShaderComponent>().UseShader(); // TODO: Attach GetComponent function to IComponent

            Gl.BindVertexArray(mesh.VAO);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mesh.VBO);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.EBO);

            Gl.DrawArrays(PrimitiveType.Triangles, 0, mesh.indexCount);

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public override void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            // TODO: Handle event based on event type and arguments
        }

        public override void Update()
        {
            // TODO: Handle game update (no logic here?)
        }
    }
}
