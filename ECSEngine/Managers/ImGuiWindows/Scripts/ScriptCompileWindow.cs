using ECSEngine.Assets;
using ImGuiNET;
using System;
using System.Diagnostics;
using System.Numerics;

namespace ECSEngine.Managers.ImGuiWindows.Scripts
{
    class ScriptCompileWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string Title { get; } = "Script Compilation";
        public override string IconGlyph { get; } = FontAwesome5.FileCode;

        public override void Draw()
        {
            ImGui.PushItemWidth(-1);
            ImGui.SetScrollHereY(1.0f);
            ImGui.InputTextMultiline("Console", ref Debug.pastLogsStringConsole, UInt32.MaxValue, new Vector2(-1, -1), ImGuiInputTextFlags.ReadOnly);

            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            {
                Process ideProcess = new Process();
                ideProcess.StartInfo = new ProcessStartInfo("code", "Content/");
                ideProcess.Start();
            }
            ImGui.PopItemWidth();
        }
    }
}
