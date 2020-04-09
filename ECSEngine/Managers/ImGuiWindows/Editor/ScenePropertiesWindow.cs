using ECSEngine.Assets;
using ECSEngine.Entities;
using ImGuiNET;

namespace ECSEngine.Managers.ImGuiWindows.Editor
{
    class ScenePropertiesWindow : IImGuiWindow
    {
        public bool Render { get; set; }
        public string IconGlyph { get; } = FontAwesome5.Sitemap;
        public string Title { get; } = "Scene Properties";

        private int currentSceneHierarchyItem;
        private IEntity selectedEntity;

        public void Draw()
        {
            var entityNames = new string[SceneManager.Instance.Entities.Count];
            for (var i = 0; i < SceneManager.Instance.Entities.Count; i++)
            {
                entityNames[i] = SceneManager.Instance.Entities[i].GetType().Name;
            }

            ImGui.PushItemWidth(-1);
            if (ImGui.ListBox("Hierarchy", ref currentSceneHierarchyItem, entityNames, entityNames.Length))
            {
                selectedEntity = SceneManager.Instance.Entities[currentSceneHierarchyItem];
            }
            ImGui.PopItemWidth();

            ImGui.Separator();

            if (selectedEntity == null)
            {
                ImGui.End();
                return;
            }

            selectedEntity.RenderImGui();
        }
    }
}
