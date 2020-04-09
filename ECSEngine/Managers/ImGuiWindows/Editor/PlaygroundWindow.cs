﻿using ECSEngine.Assets;
using ImGuiNET;
using System;

namespace ECSEngine.Managers.ImGuiWindows.Editor
{
    class PlaygroundWindow : IImGuiWindow
    {
        public bool Render { get; set; }
        public string IconGlyph { get; } = FontAwesome5.Play;
        public string Title { get; } = "Playground";

        public void Draw()
        {
            // Loading test
            var progress = (int)(ImGui.GetTime() / 0.025f) % 100;
            var filesLoaded = (int)Math.Round(progress * 0.25);
            ImGui.Text($"Loading, please wait... {@"|/-\"[(int)(ImGui.GetTime() / 0.25f) % 4]}");
            ImGui.TextUnformatted($"Files loaded: {progress}% ({filesLoaded} / 25)"); // TextUnformatted displays '%' without needing to format it as '%%'.
            ImGui.TextUnformatted($"Test " + FontAwesome5.Egg);
        }
    }
}
