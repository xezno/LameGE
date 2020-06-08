using Engine.ECS.Components;
using Engine.Renderer.GL.Render;
using Engine.Utils.Attributes;

namespace Engine.Renderer.GL.Components
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

        public MeshComponent(Mesh mesh)
        {
            RenderMesh = mesh;
        }

        /// <summary>
        /// Construct a blank mesh component, with no loaded mesh.
        /// </summary>
        public MeshComponent()
        {
            RenderMesh = new Mesh();
        }
    }
}
