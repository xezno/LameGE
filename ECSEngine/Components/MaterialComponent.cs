using System.Collections.Generic;

using ECSEngine.Attributes;
using ECSEngine.Render;

namespace ECSEngine.Components
{
    [Requires(typeof(ShaderComponent))]
    public class MaterialComponent : Component<MaterialComponent>
    {
        private List<Material> materials = new List<Material>();

        public MaterialComponent(List<Material> materials)
        {
            this.materials = materials;
        }

        public void AddMaterial(Material newMaterial)
        {
            materials.Add(newMaterial);
        }

        public void AddMaterials(List<Material> newMaterials)
        {
            foreach (Material newMaterial in newMaterials)
            {
                AddMaterial(newMaterial);
            }
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
