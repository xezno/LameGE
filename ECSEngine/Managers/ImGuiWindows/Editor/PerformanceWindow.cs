using ECSEngine.Assets;
using ImGuiNET;

namespace ECSEngine.Managers.ImGuiWindows.Editor
{
    class PerformanceWindow : IImGuiWindow
    {
        public bool Render { get; set; }
        public string IconGlyph { get; } = FontAwesome5.ChartLine;
        public string Title { get; } = "Performance";

        public void Draw()
        {
            ImGui.LabelText($"{RenderManager.Instance.LastFrameTime}ms", "Current frametime");

            ImGui.PlotHistogram(
                "Average frame time",
                ref RenderManager.Instance.FrametimeHistory[0],
                RenderManager.Instance.FrametimeHistory.Length,
                0,
                "",
                0
            );

            ImGui.LabelText($"{RenderManager.Instance.CalculatedFramerate}fps", "Current framerate");
            ImGui.PlotLines(
                "Average frame rate",
                ref RenderManager.Instance.FramerateHistory[0],
                RenderManager.Instance.FramerateHistory.Length,
                0,
                "",
                0
            );
        }
    }
}
