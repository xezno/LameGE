using ECSEngine.Assets;
using ImGuiNET;
using System;
using System.Numerics;

namespace ECSEngine.Managers.ImGuiWindows.Editor
{
    class ConsoleWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string IconGlyph { get; } = FontAwesome5.Terminal;
        public override string Title { get; } = "Console";

        private string currentConsoleFilter = "", currentConsoleInput = "";

        public override void Draw()
        {
            ImGui.PushItemWidth(-1);
            ImGui.SetScrollHereY(1.0f);
            ImGui.InputTextMultiline("Console", ref Debug.pastLogsStringConsole, UInt32.MaxValue, new Vector2(-1, -52), ImGuiInputTextFlags.ReadOnly);
            ImGui.PopItemWidth();

            if (ImGui.InputText("Filter", ref currentConsoleFilter, 256))
            {
                Debug.CalcLogStringByFilter(currentConsoleFilter);
            }

            ImGui.InputText("Input", ref currentConsoleInput, 256);
        }
    }
}
