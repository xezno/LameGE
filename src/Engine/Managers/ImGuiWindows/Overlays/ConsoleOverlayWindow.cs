using Engine.Assets;
using System.Numerics;

namespace Engine.Managers.ImGuiWindows.Overlays
{
    class ConsoleOverlayWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;
        public override string IconGlyph { get; } = FontAwesome5.Question;
        public override string Title { get; } = "Console Overlay";

        public override void Draw()
        {
            DrawShadowLabel(DebugUtils.Logging.PastLogsString, new Vector2(8, 8));
        }
    }
}
