using Engine.Assets;
using Engine.Components;
using Engine.Renderer.GL.Managers;
using ImGuiNET;
using System;

namespace Engine.Gui.Managers.ImGuiWindows.Editor
{
    class ViewportWindow : ImGuiWindow
    {
        public override bool Render { get; set; }
        public override string IconGlyph { get; } = FontAwesome5.Play;
        public override string Title { get; } = "Viewport";

        public override void Draw()
        {
            var windowWidth = ImGui.GetWindowSize().X;
            var camera = SceneManager.Instance.MainCamera.GetComponent<CameraComponent>();
            var ratio = camera.Resolution.y / camera.Resolution.x;
            var image = camera.Framebuffer.ColorTexture;

            ImGui.TextUnformatted("Scene");
            ImGui.Image((IntPtr)image, new System.Numerics.Vector2(windowWidth, windowWidth * ratio), new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));

        }
    }
}
