using ECSEngine.Attributes;
using ECSEngine.Render;

namespace ECSEngine.Components
{
    /// <summary>
    /// A container for various materials that may be rendered on an entity.
    /// </summary>
    [Requires(typeof(ShaderComponent))]
    public class MaterialComponent : Component<MaterialComponent>
    {
        /// <summary>
        /// An array of materials that may be rendered.
        /// </summary>
        private readonly Material[] materials;

        /// <summary>
        /// Construct the <see cref="MaterialComponent"/>, with any materials required.
        /// </summary>
        /// <param name="materials">Any number of materials to be used.</param>
        public MaterialComponent(params Material[] materials)
        {
            this.materials = materials;
        }

        /// <summary>
        /// Bind all of the materials to the current OpenGL context, and set all relevant <see cref="ShaderComponent"/> variables.
        /// </summary>
        /// <param name="shaderComponent">The relevant <see cref="ShaderComponent"/> where relevant variables will be assigned a value.</param>
        public void BindAll(ShaderComponent shaderComponent)
        {
            // TODO: Bind all material variables
            // TODO: Select material based on mesh used (+ require meshcomponent)
            materials[0].diffuseTexture?.Bind();
            foreach (var field in typeof(Material).GetFields())
            {
                shaderComponent.SetVariable($"material.{field.Name}", field.GetValue(materials[0]));
            }
        }
    }
}
