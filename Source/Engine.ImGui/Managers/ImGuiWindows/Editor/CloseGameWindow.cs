﻿using Engine.Assets;
using Engine.Events;
using Engine.Utils;
using Engine.Utils.DebugUtils;
using ImGuiNET;
using OpenGL.CoreUI;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
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
                Logging.Log("Close game");

            ImGui.SameLine();

            if (ImGui.Button("No"))
                Render = false;

            ImGui.SetWindowSize(new Vector2(0, 0));
            var windowSize = ImGui.GetWindowSize();
            var windowPos = (new Vector2(GameSettings.GameResolutionX, GameSettings.GameResolutionY) / 2f) - (windowSize / 2f);
            ImGui.SetWindowPos(windowPos);
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