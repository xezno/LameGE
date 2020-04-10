using ECSEngine;
using ECSEngine.Managers;
using ECSEngine.Managers.ImGuiWindows;
using ImGuiNET;

namespace UlaidGame.Scripts
{
    class TestScript
    {
        public int Number { get; private set; } = 12;

        public void Run()
        {
            Debug.Log($"abracadabra; Number is {++Number}", Debug.Severity.Fatal);

            ImGuiManager.Instance.Overlays.Add(new TestWindow());
        }
    }

    class TestWindow : IImGuiWindow
    {
        public bool Render { get; set; } = true;
        public string Title { get; }
        public string IconGlyph { get; }

        public void Draw()
        {
            ImGui.Begin("Test Window");
            ImGui.Button("Test 1234");
            ImGui.End();
        }
    }
}
