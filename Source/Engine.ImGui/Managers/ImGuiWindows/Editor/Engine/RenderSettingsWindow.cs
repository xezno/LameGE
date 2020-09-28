using Engine.Assets;
using ImGuiNET;
using Quincy.Managers;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    class RenderSettingsWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string IconGlyph { get; } = FontAwesome5.DrawPolygon;
        public override string Title { get; } = "Render Settings";

        public override void Draw()
        {
            ImGui.SliderFloat("Exposure", ref RenderManager.Instance.exposure, 0.0f, 10.0f, "%.2f", ImGuiSliderFlags.Logarithmic);
            ImGui.InputText("HDRI", ref RenderManager.Instance.hdri, 256, ImGuiInputTextFlags.None);
        }
    }
}
