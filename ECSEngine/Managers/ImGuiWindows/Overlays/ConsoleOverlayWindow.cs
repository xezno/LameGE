using ECSEngine.Assets;
using ImGuiNET;
using System.Numerics;

namespace ECSEngine.Managers.ImGuiWindows.Overlays
{
    class ConsoleOverlayWindow : IImGuiWindow
    {
        public bool Render { get; set; } = true;
        public string IconGlyph { get; } = FontAwesome5.Question;
        public string Title { get; } = "Console Overlay";

        public void Draw()
        {
            var consoleText = Debug.PastLogsString;
            var consoleTextPos = new Vector2(8, 8);

            ImGui.GetBackgroundDrawList().AddText(
                consoleTextPos + new Vector2(1, 1), 0x88000000, consoleText); // Shadow
            ImGui.GetBackgroundDrawList().AddText(
                consoleTextPos, 0xFFFFFFFF, consoleText);
        }
    }
}
