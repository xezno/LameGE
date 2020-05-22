using Engine.Assets;
using Engine.Utils.DebugUtils;
using ImGuiNET;

namespace Engine.Gui.Managers.ImGuiWindows.Overlays
{
    class ConsoleOverlayWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;
        public override string IconGlyph { get; } = FontAwesome5.Question;
        public override string Title { get; } = "Console Overlay";

        public override void Draw()
        {
            DrawShadowLabel(Logging.PastLogsString, ImGui.GetStyle().WindowPadding);
        }
    }
}
