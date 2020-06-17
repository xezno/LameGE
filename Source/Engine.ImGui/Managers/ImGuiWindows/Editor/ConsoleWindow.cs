using Engine.Assets;
using Engine.ECS.Notify;
using Engine.Utils.DebugUtils;
using ImGuiNET;
using OpenGL.CoreUI;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    class ConsoleWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;
        public override string IconGlyph { get; } = FontAwesome5.Terminal;
        public override string Title { get; } = "Console";

        private string currentConsoleFilter = "", currentConsoleInput = "";

        public override void Draw()
        {
            ImGui.PushItemWidth(-1);
            ImGui.SetScrollHereY(1.0f);
            ImGui.InputTextMultiline("Console", ref Logging.pastLogsStringConsole, uint.MaxValue, new Vector2(-1, -64), ImGuiInputTextFlags.ReadOnly);
            ImGui.PopItemWidth();

            if (ImGui.InputText("Filter", ref currentConsoleFilter, 256))
            {
                Logging.CalcLogStringByFilter(currentConsoleFilter);
            }

            ImGui.InputText("##hidelabel", ref currentConsoleInput, 256);
            ImGui.SameLine();
            ImGui.Button("Submit");
        }

        public override void OnNotify(NotifyType eventType, INotifyArgs notifyArgs)
        {
            // TODO: Event not ever fired
            if (eventType == NotifyType.KeyUp)
            {
                if (((KeyboardNotifyArgs)notifyArgs).KeyboardKey == (int)KeyCode.F3)
                {
                    Render = !Render;
                }
            }
        }
    }
}
