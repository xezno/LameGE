using Engine.Assets;
using Engine.Renderer.GL.Managers;
using ImGuiNET;
using System;
using System.Linq;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    class TextureBrowserWindow : ImGuiWindow
    {
        public override string Title { get; } = "Texture Browser";
        public override string IconGlyph { get; } = FontAwesome5.Square;
        public override bool Render { get; set; }

        private int selectedTexture;

        public override void Draw()
        {
            var textureList = AssetManager.Instance.Textures;

            ImGui.Combo("Texture", ref selectedTexture, textureList.Keys.ToArray(), textureList.Count);

            var texture = textureList.Values.ToArray()[selectedTexture];

            var windowWidth = ImGui.GetWindowSize().X;
            var ratio = (float)texture.height / texture.width;

            ImGui.Text($"Texture selected: {texture.glTexture}");
            ImGui.Image((IntPtr)texture.glTexture, new Vector2(windowWidth, windowWidth * ratio), new Vector2(0, 1), new Vector2(1, 0));
        }
    }
}
