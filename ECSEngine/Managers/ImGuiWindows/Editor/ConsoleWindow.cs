using ECSEngine.Assets;
using ImGuiNET;
using System;
using System.Numerics;

namespace ECSEngine.Managers.ImGuiWindows.Editor
{
    class ConsoleWindow : IImGuiWindow
    {
        public bool Render { get; set; }
        public string IconGlyph { get; } = FontAwesome5.Terminal;
        public string Title { get; } = "Console";

        private string currentConsoleFilter = "", currentConsoleInput = "";

        public void Draw()
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
