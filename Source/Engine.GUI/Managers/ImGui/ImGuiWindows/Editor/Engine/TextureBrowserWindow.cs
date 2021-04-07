using Engine.Assets;
using Engine.Renderer;
using ImGuiNET;
using System;
using System.Numerics;

namespace Engine.GUI.Managers.ImGuiWindows.Editor
{
    [ImGuiMenuPath(ImGuiMenus.Menu.Engine)]
    class TextureBrowserWindow : ImGuiWindow
    {
        public override string Title { get; } = "Texture Browser";
        public override string IconGlyph { get; } = FontAwesome5.Square;
        public override bool Render { get; set; }

        private int selectedTexture;

        public override void Draw()
        {
            var textureList = TextureContainer.Textures;
            var texturePaths = TextureContainer.TexturePaths;

            ImGui.Combo("Texture", ref selectedTexture, texturePaths.ToArray(), texturePaths.Count);

            if (selectedTexture < 0)
                selectedTexture = 0;

            if (selectedTexture > textureList.Count - 1)
                selectedTexture = textureList.Count - 1;

            var texture = textureList[selectedTexture];

            var windowWidth = ImGui.GetWindowSize().X;
            var ratio = 1f;

            ImGui.Text($"Texture selected: {texture.Id}");
            ImGui.Image((IntPtr)texture.Id, new Vector2(windowWidth, windowWidth * ratio), new Vector2(0, 1), new Vector2(1, 0));
        }
    }
}
