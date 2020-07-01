using Engine.Assets;
using Engine.ECS.Notify;
using Engine.Utils.DebugUtils;
using ImGuiNET;
using OpenGL.CoreUI;
using System.Linq;
using System.Numerics;
using static Engine.Utils.DebugUtils.Logging;

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
            // ImGui.PushStyleColor(ImGuiCol.ChildBg, ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg]);
            ImGui.BeginChild("ConsoleInner", new Vector2(-1, -64));
            ImGui.PushFont(ImGuiManager.Instance.MonospacedFont);

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

                ImGui.PushStyleColor(ImGuiCol.Text, SeverityToColor(logEntry.severity));
                ImGui.TextWrapped(logEntry.ToString());
                ImGui.PopStyleColor();

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetNextWindowSize(new Vector2(650, -1));

                    ImGui.BeginTooltip();

                    ImGui.Text("Stack Trace:");
                    ImGui.TextWrapped(logEntry.stackTrace.ToString());

                    ImGui.EndTooltip();
                }

                ImGui.Separator();
            }

            if (scrollQueued)
            {
                ImGui.SetScrollHereY(1.0f);
                scrollQueued = false;
            }
            
            ImGui.PopFont();
            ImGui.EndChild();
            // ImGui.PopStyleColor();
            
            ImGui.InputText("Filter", ref currentConsoleFilter, 256);
            // ImGui.InputText("##hidelabel", ref currentConsoleInput, 256); // TODO
            // ImGui.SameLine();
            // ImGui.Button("Submit");
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

        public static Vector4 SeverityToColor(Severity severity)
        {
            switch (severity)
            {
                case Severity.Fatal:
                    return new Vector4(255f / 255f, 0, 0, 1);
                case Severity.High:
                    return new Vector4(255f / 255f, 94f / 255f, 94f / 255f, 1);
                case Severity.Low:
                    return new Vector4(67f / 255f, 255f / 255f, 83f / 255f, 1);
                case Severity.Medium:
                    return new Vector4(255f / 255f, 200f / 255f, 0, 1);
            }
            
            return new Vector4(67f / 255f, 255f / 255f, 83f / 255f, 1);
        }
    }
}
