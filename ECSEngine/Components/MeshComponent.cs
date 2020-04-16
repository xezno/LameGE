using ECSEngine.Attributes;
using ECSEngine.Entities;
using ECSEngine.Managers;
using ECSEngine.MathUtils;
using ECSEngine.Render;
using OpenGL;

namespace ECSEngine.Components
{
    /// <summary>
    /// Contains the component-based code used in order to render
    /// a mesh on-screen.
    /// REQUIRES:
    ///  - <see cref="MaterialComponent " />
    ///  - <see cref="ShaderComponent   ">
    ///  - <see cref="TransformComponent">
    /// </summary>
    [Requires(typeof(MaterialComponent))]
    [Requires(typeof(ShaderComponent))]
    [Requires(typeof(TransformComponent))]
    public class MeshComponent : Component<MeshComponent>
    {
        /// <summary>
        /// The <see cref="Mesh"/> to draw.
        /// </summary>
        public Mesh RenderMesh { get; }

        private ShaderComponent shaderComponent;
        private TransformComponent transformComponent;

        /// <summary>
        /// Construct an instance of MeshComponent, loading the mesh from the path specified.
        /// <para>REQUIRES:</para>
        /// <para> - <see cref="MaterialComponent"/></para>
        /// <para> - <see cref="ShaderComponent"/></para>
        /// <para> - <see cref="TransformComponent"/></para>
        /// </summary>
        /// <param name="path">The path to load the <see cref="Mesh"/> from.</param>
        public MeshComponent(string path)
        {
            RenderMesh = new Mesh(path);
        }

        /// <summary>
        /// Construct a blank mesh component, with no loaded mesh.
        /// </summary>
        public MeshComponent()
        {
            RenderMesh = new Mesh();
        }

        /// <summary>
        /// Draw the <see cref="Mesh"/> on-screen using the attached <see cref="ShaderComponent"/> and <see cref="MaterialComponent"/>.
        /// </summary>
        public override void Render()
        {
            shaderComponent = ((IEntity)Parent).GetComponent<ShaderComponent>();
            transformComponent = ((IEntity)Parent).GetComponent<TransformComponent>();

            shaderComponent.UseShader(); // TODO: Attach GetComponent function to IComponent

            Gl.BindVertexArray(RenderMesh.vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, RenderMesh.vbo);

            var camera = ((SceneManager)Parent.Parent).mainCamera;
            shaderComponent.SetVariable("projMatrix", camera.ProjMatrix);
            shaderComponent.SetVariable("viewMatrix", camera.ViewMatrix);
            shaderComponent.SetVariable("cameraPos", camera.Position);
            shaderComponent.SetVariable("modelMatrix", transformComponent.Matrix);
            shaderComponent.SetVariable("fogNear", 0.02f);
            shaderComponent.SetVariable("skyColor", new Vector3(100 / 255f, 149 / 255f, 237 / 255f)); // Cornflower blue

            GetComponent<MaterialComponent>().BindAll(shaderComponent);

            SceneManager.Instance.lights[0].Bind(shaderComponent);

            Gl.DrawArrays(PrimitiveType.Triangles, 0, RenderMesh.ElementCount * sizeof(float));

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
