﻿using Engine.Assets;
using Engine.ECS.Observer;
using Engine.Utils;
using Engine.Utils.DebugUtils;
using ImGuiNET;
using System.Numerics;

namespace Engine.GUI.Managers.ImGuiWindows.Editor
{
    [ImGuiMenuPath(ImGuiMenus.Menu.File)]
    class CloseGameWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string IconGlyph { get; } = FontAwesome5.WindowClose;
        public override string Title { get; } = "Quit";
        public override ImGuiWindowFlags Flags { get; } = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoDecoration;

        public override void Draw()
        {
            ImGui.Text("Are you sure you want to quit?");

            if (ImGui.Button("Yes"))
            {
                Subject.Notify(NotifyType.CloseGame, null);
                Logging.Log("Close game");
            }

            ImGui.SameLine();

            if (ImGui.Button("No"))
                Render = false;

            ImGui.SetWindowSize(new Vector2(0, 0));
            var windowSize = ImGui.GetWindowSize();
            var windowPos = (new Vector2(GameSettings.GameResolutionX, GameSettings.GameResolutionY) / 2f) - (windowSize / 2f);
            ImGui.SetWindowPos(windowPos);
        }
    }
}
