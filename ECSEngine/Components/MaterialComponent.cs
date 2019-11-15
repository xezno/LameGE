using System.Collections.Generic;

using ECSEngine.Attributes;
using ECSEngine.Render;

namespace ECSEngine.Components
{
    [Requires(typeof(ShaderComponent))]
    public class MaterialComponent : Component<MaterialComponent>
    {
        private Material[] materials;

        public MaterialComponent(params Material[] materials)
        {
            this.materials = materials;
        }

        public void BindAll(ShaderComponent shaderComponent)
        {
            // TODO: Bind all material variables
            // TODO: Select material based on mesh used (+ require meshcomponent)
            materials[0].diffuseTexture.BindTexture();
            foreach (var field in typeof(Material).GetFields())
            {
                shaderComponent.SetVariable(field.Name, field.GetValue(materials[0]));
            }
        }
    }
}
