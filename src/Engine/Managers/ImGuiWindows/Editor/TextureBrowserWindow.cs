using Engine.Assets;
using ImGuiNET;
using System;
using System.Numerics;

namespace Engine.Managers.ImGuiWindows.Editor
{
    class TextureBrowserWindow : ImGuiWindow
    {
        public override string Title { get; } = "Texture Browser";
        public override string IconGlyph { get; } = FontAwesome5.Square;
        public override bool Render { get; set; } = true;
        private int selectedTextureUnit = 0;

        // this does not work
        public override void Draw()
        {
            ImGui.SliderInt("Texture ptr", ref selectedTextureUnit, 0, 128);
            ImGui.Text($"Texture selected: {(IntPtr)selectedTextureUnit}");
            ImGui.Image((IntPtr)selectedTextureUnit, new Vector2(128, 128));
        }
    }
}
