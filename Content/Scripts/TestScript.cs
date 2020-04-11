using ECSEngine;
using ECSEngine.Assets;
using ECSEngine.Managers;
using ECSEngine.Managers.ImGuiWindows;
using ImGuiNET;
using System.Linq;

namespace UlaidGame.Scripts
{
    class ScriptPreviewWindowInjector
    {
        public void Run()
        {
            Debug.Log("Hello, world!");
            ImGuiManager.Instance.Overlays.Add(new ScriptPreviewWindow());
        }

        public void Unload() { }
    }

    class ScriptPreviewWindow : IImGuiWindow
    {
        public bool Render { get; set; } = true;
        public string Title { get; } = "Scripts";
        public string IconGlyph { get; } = FontAwesome5.FileCode;

        public void Draw()
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
    }
}
