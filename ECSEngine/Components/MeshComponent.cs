using ECSEngine.Attributes;
using ECSEngine.Entities;
using ECSEngine.Events;
using ECSEngine.Render;
using ECSEngine.Systems;
using OpenGL;
using System;

namespace ECSEngine.Components
{
    /// <summary>
    /// Contains the component-based code used in order to render
    /// a mesh on-screen.
    /// </summary>
    [Requires(typeof(ShaderComponent))]
    public class MeshComponent : Component<MeshComponent>
    {
        Matrix4x4f modelMatrix;
        Mesh mesh;
        public MeshComponent(string path)
        {
            mesh = new Mesh(path);
            modelMatrix = Matrix4x4f.Identity;
            modelMatrix.Scale(
                0.5f,
                0.5f,
                0.5f);
            modelMatrix.RotateX(90f);
        }
        
        public override void Render()
        {
            ShaderComponent shaderComponent = ((IEntity)parent).GetComponent<ShaderComponent>();
            shaderComponent.UseShader(); // TODO: Attach GetComponent function to IComponent

            Gl.BindVertexArray(mesh.VAO);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mesh.VBO);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.EBO);

            CameraEntity camera = ((WorldSystem)parent.parent).mainCamera;
            shaderComponent.SetVariable("projMatrix", camera.projMatrix);
            shaderComponent.SetVariable("viewMatrix", camera.viewMatrix);
            shaderComponent.SetVariable("modelMatrix", modelMatrix);

            Gl.DrawElements(PrimitiveType.Triangles, mesh.indexCount * sizeof(uint), DrawElementsType.UnsignedInt, IntPtr.Zero);
            // Gl.DrawArrays(PrimitiveType.Triangles, 0, mesh.vertexCount * sizeof(float));

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
