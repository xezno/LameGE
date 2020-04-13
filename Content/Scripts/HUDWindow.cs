using ECSEngine;
using ECSEngine.Assets;
using ECSEngine.Managers;
using ECSEngine.Managers.ImGuiWindows;
using ECSEngine.Managers.Scripting;
using ImGuiNET;
using System.Linq;

namespace UlaidGame.Scripts
{
    class PackageHook
    {
        public void Run()
        {
            Debug.Log("Hello, world!");
            ImGuiManager.Instance.Overlays.Add(new HUDWindow());
        }

        public void Unload() { }
    }

    class Player
    {
        public int Health { get; } = 100;
        public int Shields { get; } = 100;
    }

    class HUDWindow : IImGuiWindow
    {
        public bool Render { get; set; } = true;
        public string Title { get; } = "Scripts";
        public string IconGlyph { get; } = FontAwesome5.FileCode;

        private Player Player { get; } = new Player();

        private void DrawScriptsWindow()
        {
            ImGui.Begin("Scripts");
            for (int i = 0; i < ScriptManager.Instance.ScriptList.Count; ++i)
            {
                var scriptName = ScriptManager.Instance.ScriptList.Keys.ToArray()[i];
                ImGui.LabelText("##hidelabel", scriptName);
                ImGui.SameLine();
                if (ImGui.Button("Reload"))
                {
                    ScriptManager.Instance.ReloadScript(scriptName);
                }
            }
            ImGui.End();
        }

        private void DrawHud()
        {
            ImGui.Begin("Hud");
            ImGui.LabelText("Health", Player.Health.ToString());
            ImGui.LabelText("Shields", Player.Shields.ToString());
            ImGui.End();
        }

        public void Draw()
        {
            DrawScriptsWindow();
            DrawHud();
        }
    }
}