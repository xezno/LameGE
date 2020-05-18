using Engine.Assets;
using Engine.Events;
using Engine.Utils.DebugUtils;
using ImGuiNET;
using OpenGL.CoreUI;
using System.Numerics;

namespace Engine.Renderer.GL.Managers.ImGuiWindows.Editor
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

        public override void HandleEvent(Event eventType, IEventArgs baseEventArgs)
        {
            // TODO: Event not ever fired
            if (eventType == Event.KeyUp)
            {
                if (((KeyboardEventArgs)baseEventArgs).KeyboardKey == (int)KeyCode.F3)
                {
                    Render = !Render;
                }
            }
        }
    }
}
