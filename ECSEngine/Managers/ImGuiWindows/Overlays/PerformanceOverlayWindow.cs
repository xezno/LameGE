using ECSEngine.Assets;
using ImGuiNET;
using System.Numerics;

namespace ECSEngine.Managers.ImGuiWindows.Overlays
{
    class PerformanceOverlayWindow : IImGuiWindow
    {
        public bool Render { get; set; } = true;
        public string IconGlyph { get; } = FontAwesome5.Question;
        public string Title { get; } = "Performance Overlay";

        public void Draw()
        {
            var debugText = FontAwesome5.Poop + " Engine\n" +
                            "F1 for editor\n" +
                            $"{RenderManager.Instance.LastFrameTime}ms\n" +
                            $"{RenderManager.Instance.CalculatedFramerate}fps";

            var debugTextPos = new Vector2(RenderSettings.Default.gameResolutionX - 128, 8);

            ImGui.GetBackgroundDrawList().AddText(
                debugTextPos + new Vector2(1, 1), 0x88000000, debugText); // Shadow

            ImGui.GetBackgroundDrawList().AddText(
                debugTextPos, 0xFFFFFFFF, debugText);
        }
    }
}
