﻿using Engine.Assets;
using Engine.Utils;
using System.Numerics;

namespace Engine.Renderer.GL.Managers.ImGuiWindows.Overlays
{
    class PerformanceOverlayWindow : ImGuiWindow
    {
        public override bool Render { get; set; } = true;
        public override string IconGlyph { get; } = FontAwesome5.Question;
        public override string Title { get; } = "Performance Overlay";

        public override void Draw()
        {
            var debugText = FontAwesome5.Poop + " Engine\n" +
                            "F1 for editor\n" +
                            $"{RenderManager.Instance.LastFrameTime}ms\n" +
                            $"{RenderManager.Instance.CalculatedFramerate}fps";

            DrawShadowLabel(debugText, new Vector2(GameSettings.GameResolutionX - 128, 8));
        }
    }
}