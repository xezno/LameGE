using Engine.Assets;
using Engine.ECS.Notify;
using Engine.Utils.DebugUtils;
using ImGuiNET;
using OpenGL.CoreUI;
using System.Linq;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    class ConsoleWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;
        public override string IconGlyph { get; } = FontAwesome5.Terminal;
        public override string Title { get; } = "Console";

        private string currentConsoleFilter = "", currentConsoleInput = "";

        private int logLimit = 1024;

        private bool scrollQueued = true;

        public ConsoleWindow()
        {
            Logging.onDebugLog += (entry) =>
            {
                scrollQueued = true;
            };
        }

        public override void Draw()
        {
            ImGui.BeginChild("ConsoleInner", new Vector2(-1, -64));

            foreach (var logEntry in Logging.LogEntries.TakeLast(logLimit))
            {
                bool drawCurrentEntry = true;
                
                if (!string.IsNullOrWhiteSpace(currentConsoleFilter) && !GetFilterMatch(logEntry.ToString(), currentConsoleFilter))
                {
                    drawCurrentEntry = false;
                }

                if (!drawCurrentEntry)
                {
                    continue;
                }

                ImGui.TextWrapped(logEntry.ToString());

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("Stack Trace:");
                    ImGui.Text(logEntry.stackTrace.ToString());
                    ImGui.EndTooltip();
                }

                ImGui.Separator();
            }

            if (scrollQueued)
            {
                ImGui.SetScrollHereY(1.0f);
                scrollQueued = false;
            }

            ImGui.EndChild();
            
            ImGui.InputText("Filter", ref currentConsoleFilter, 256);
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

        // TODO: use fuzzy search
        private static bool GetFilterMatch(string str, string filter)
        {
            if (filter.Contains(","))
            {
                foreach (var splitFilter in filter.Split(','))
                {
                    if (str.Contains(splitFilter))
                        return true;
                }

                return false;
            }
            else
            {
                return str.Contains(filter);
            }
        }
    }
}
