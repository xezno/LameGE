using System;
using System.Diagnostics;
using System.Numerics;
using ECSEngine.Assets;
using ImGuiNET;

namespace ECSEngine.Managers.ImGuiWindows.Scripts
{
    class ScriptCompileWindow : IImGuiWindow
    {
        public bool Render { get; set; }
        public string Title { get; } = "Script Compilation";
        public string IconGlyph { get; } = FontAwesome5.FileCode;

        public void Draw()
        {
            ImGui.PushItemWidth(-1);
            ImGui.SetScrollHereY(1.0f);
            ImGui.InputTextMultiline("Console", ref Debug.pastLogsStringConsole, UInt32.MaxValue, new Vector2(-1, -1), ImGuiInputTextFlags.ReadOnly);

            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            {
                Process ideProcess = new Process();
                ideProcess.StartInfo = new ProcessStartInfo("code Content/");
                ideProcess.Start();
            }
            ImGui.PopItemWidth();
        }
    }
}
