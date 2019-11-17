using ECSEngine.Attributes;
using ECSEngine.Entities;
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
        /// <summary>
        /// The matrix to apply to the <see cref="mesh"/> upon drawing it.
        /// </summary>
        private readonly Matrix4x4f modelMatrix;

        /// <summary>
        /// The <see cref="Mesh"/> to draw.
        /// </summary>
        private readonly Mesh mesh;

        /// <summary>
        /// Construct an instance of MeshComponent, loading the mesh from the path specified.
        /// </summary>
        /// <param name="path">The path to load the <see cref="Mesh"/> from.</param>
        public MeshComponent(string path)
        {
            mesh = new Mesh(path);
            modelMatrix = Matrix4x4f.Identity;
        }

        /// <summary>
        /// Draw the <see cref="Mesh"/> on-screen using the attached <see cref="ShaderComponent"/> and <see cref="MaterialComponent"/>.
        /// </summary>
        public override void Render()
        {
            ShaderComponent shaderComponent = ((IEntity)parent).GetComponent<ShaderComponent>();
            shaderComponent.UseShader(); // TODO: Attach GetComponent function to IComponent

            Gl.BindVertexArray(mesh.VAO);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mesh.VBO);

            GetComponent<MaterialComponent>().BindAll(shaderComponent);

            CameraEntity camera = ((WorldSystem)parent.parent).mainCamera;
            shaderComponent.SetVariable("projMatrix", camera.projMatrix);
            shaderComponent.SetVariable("viewMatrix", camera.viewMatrix);
            shaderComponent.SetVariable("modelMatrix", modelMatrix);
            shaderComponent.SetVariable("albedoTexture", 0);

            // Gl.DrawElements(PrimitiveType.Triangles, mesh.indexCount * sizeof(uint), DrawElementsType.UnsignedInt, IntPtr.Zero);
            Gl.DrawArrays(PrimitiveType.Triangles, 0, mesh.elementCount * sizeof(float));

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
