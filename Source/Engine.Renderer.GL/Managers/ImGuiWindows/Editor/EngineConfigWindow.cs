using Engine.Assets;
using ImGuiNET;
using System;

namespace Engine.Renderer.GL.Managers.ImGuiWindows.Editor
{
    class EngineConfigWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string IconGlyph { get; } = FontAwesome5.Cog;
        public override string Title { get; } = "Engine Config";

        public override void Draw()
        {
            var progress = (int)(ImGui.GetTime() / 0.025f) % 100;
            var filesLoaded = (int)Math.Round(progress * 0.25);
            ImGui.Text($"Loading, please wait... {@"|/-\"[(int)(ImGui.GetTime() / 0.25f) % 4]}");
            ImGui.TextUnformatted($"Files loaded: {progress}% ({filesLoaded} / 25)"); // TextUnformatted displays '%' without needing to format it as '%%'.
            ImGui.TextUnformatted($"Test " + FontAwesome5.Egg);
        }
    }
}
