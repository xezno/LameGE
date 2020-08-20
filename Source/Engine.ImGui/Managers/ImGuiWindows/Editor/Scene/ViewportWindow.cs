using Engine.Assets;
using Engine.Components;
using Engine.Renderer.GL.Entities;
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
            ImGui.Columns(2);
            foreach (CameraEntity camera in SceneManager.Instance.Cameras)
            {
                DrawCamera(camera);
                ImGui.NextColumn();
            }
            ImGui.Columns(1);
        }

        private void DrawCamera(CameraEntity cameraEntity)
        {
            var windowWidth = ImGui.GetWindowSize().X;
            var camera = cameraEntity.GetComponent<CameraComponent>();
            var ratio = camera.Resolution.y / camera.Resolution.x;
            var image = camera.Framebuffer.ColorTexture;
            var cameraScale = 0.5f;

            ImGui.TextUnformatted(cameraEntity.Name);
            ImGui.Image((IntPtr)image, new System.Numerics.Vector2(windowWidth, windowWidth * ratio) * cameraScale, new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));
        }
    }
}
