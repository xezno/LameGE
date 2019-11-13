using ECSEngine.Attributes;
using ECSEngine.Entities;
using ECSEngine.Events;
using ECSEngine.Render;
using ECSEngine.Systems;

using OpenGL;

namespace ECSEngine.Components
{
    /// <summary>
    /// Contains the component-based code used in order to render
    /// a mesh on-screen.
    /// </summary>
    [Requires(typeof(MaterialComponent))]
    [Requires(typeof(ShaderComponent))]
    public class MeshComponent : Component<MeshComponent>
    {
        private Matrix4x4f modelMatrix;
        private Mesh mesh;

        public MeshComponent(string path, Texture2D albedoTexture)
        {
            mesh = new Mesh(path);
            modelMatrix = Matrix4x4f.Identity;
        }
        
        public override void Render()
        {
            ShaderComponent shaderComponent = ((IEntity)parent).GetComponent<ShaderComponent>();
            shaderComponent.UseShader(); // TODO: Attach GetComponent function to IComponent

            Gl.BindVertexArray(mesh.VAO);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mesh.VBO);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.EBO);

            GetComponent<MaterialComponent>().BindAll(shaderComponent);

            CameraEntity camera = ((WorldSystem)parent.parent).mainCamera;
            shaderComponent.SetVariable("projMatrix", camera.projMatrix);
            shaderComponent.SetVariable("viewMatrix", camera.viewMatrix);
            shaderComponent.SetVariable("modelMatrix", modelMatrix);
            shaderComponent.SetVariable("albedoTexture", 0);

            // Gl.DrawElements(PrimitiveType.Triangles, mesh.indexCount * sizeof(uint), DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.DrawArrays(PrimitiveType.Triangles, 0, mesh.indexCount * sizeof(float));

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
