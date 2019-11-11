using System.Collections.Generic;

using ECSEngine.Render;

namespace ECSEngine.Components
{
    public class MaterialComponent : Component<MaterialComponent>
    {
        List<Material> materials = new List<Material>();

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
    }
}
