using ECSEngine.Assets;
using ImGuiNET;
using System;
using System.Numerics;

namespace ECSEngine.Managers.ImGuiWindows.Editor
{
    class TextureBrowserWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string Title { get; } = "Texture Browser (WIP)";
        public override string IconGlyph { get; } = FontAwesome5.Square;

        private int selectedTextureUnit;

        public override void Draw()
        {
            ImGui.SliderInt("Texture ptr", ref selectedTextureUnit, 0, 128);
            ImGui.Text($"Texture selected: {(IntPtr)selectedTextureUnit}");
            ImGui.Image((IntPtr)selectedTextureUnit, new Vector2(128, 128));
        }
    }
}
