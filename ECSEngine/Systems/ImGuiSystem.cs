using System;
using System.Collections.Generic;

using ECSEngine.Entities;
using ECSEngine.Render;
using ImGuiNET;

namespace ECSEngine.Systems
{
    public class ImGuiSystem : System<ImGuiSystem>
    {
        IntPtr imGuiContext;
        public ImGuiSystem()
        {
            imGuiContext = ImGui.CreateContext();
            ImGui.SetCurrentContext(imGuiContext);
            var io = ImGui.GetIO();

            io.Fonts.AddFontDefault();

            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bpp);
            io.Fonts.SetTexID((IntPtr)1);
            Texture2D defaultFontTexture = new Texture2D(pixels, width, height, bpp);

            io.Fonts.ClearTexData();

            ImGui.StyleColorsDark();
        }

        public override void Render()
        {
            ImGui.NewFrame();
            ImGui.ShowDemoWindow();
            ImGui.End();
            ImGui.Render();
        }

        public override void Update()
        {
        }
    }
}
