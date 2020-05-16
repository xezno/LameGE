using Engine.Assets;

namespace Engine.Renderer.GL.Managers.ImGuiWindows.Editor
{
    class RenderSettingsWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string IconGlyph { get; } = FontAwesome5.DrawPolygon;
        public override string Title { get; } = "Render Settings";

        public override void Draw()
        {
            // ImGui.SliderFloat("Exposure", )
        }
    }
}
