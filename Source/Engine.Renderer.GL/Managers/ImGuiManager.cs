using Engine.Assets;
using Engine.ECS.Managers;
using Engine.Events;
using Engine.Managers;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Managers.ImGuiWindows;
using Engine.Renderer.GL.Managers.ImGuiWindows.Editor;
using Engine.Renderer.GL.Managers.ImGuiWindows.Overlays;
using Engine.Renderer.GL.Managers.ImGuiWindows.Scripts;
using Engine.Renderer.GL.Render;
using Engine.Utils;
using Engine.Utils.DebugUtils;
using ImGuiNET;
using OpenGL;
using OpenGL.CoreUI;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vector4 = Engine.Utils.MathUtils.Vector4;

namespace Engine.Renderer.GL.Managers
{
    public class ImGuiManager : Manager<ImGuiManager>
    {
        private const float ptToPx = 1.3281472327365f;
        private Texture2D defaultFontTexture;
        private ShaderComponent shaderComponent;
        private Vector2 windowSize;
        private ImGuiIOPtr io;

        private uint vbo, vao, ebo;
        private bool showEditor;
        private bool lockCursor;

        public List<ImGuiMenu> Menus { get; } = new List<ImGuiMenu>()
        {
            new ImGuiMenu(FontAwesome5.File, "File", new List<ImGuiWindow>()),
            new ImGuiMenu(FontAwesome5.FileCode, "Scripts", new List<ImGuiWindow>()
            {
                new ScriptCompileWindow()
            }),
            new ImGuiMenu(FontAwesome5.Cogs, "Editor", new List<ImGuiWindow>()
            {
                new PlaygroundWindow(),
                new ScenePropertiesWindow(),
                new ConsoleWindow(),
                new PerformanceWindow(),
                new TextureBrowserWindow(),
                new ShaderWindow()
            })
        };

        public List<ImGuiWindow> Overlays { get; } = new List<ImGuiWindow>()
        {
            new ConsoleOverlayWindow(),
            new PerformanceOverlayWindow()
        };

        public ImGuiManager()
        {
            var imGuiContext = ImGui.CreateContext();
            if (ImGui.GetCurrentContext() == IntPtr.Zero)
                ImGui.SetCurrentContext(imGuiContext);
            io = ImGui.GetIO();

            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

            ImGui.StyleColorsDark();
            // SetupVguiStyle();
            SetupModernStyle();

            InitFonts();
            InitKeymap();
            InitGl();
        }

        private void SetupModernStyle()
        {
            var colors = ImGui.GetStyle().Colors;
            colors[(int)ImGuiCol.Text]                   = new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled]           = new System.Numerics.Vector4(0.26f, 0.26f, 0.26f, 1.00f);
            colors[(int)ImGuiCol.WindowBg]               = new System.Numerics.Vector4(0.04f, 0.04f, 0.04f, 1.00f);
            colors[(int)ImGuiCol.ChildBg]                = new System.Numerics.Vector4(0.04f, 0.04f, 0.04f, 1.00f);
            colors[(int)ImGuiCol.PopupBg]                = new System.Numerics.Vector4(0.04f, 0.04f, 0.04f, 1.00f);
            colors[(int)ImGuiCol.Border]                 = new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.00f);
            colors[(int)ImGuiCol.BorderShadow]           = new System.Numerics.Vector4(0.92f, 0.91f, 0.88f, 0.00f);
            colors[(int)ImGuiCol.FrameBg]                = new System.Numerics.Vector4(0.09f, 0.09f, 0.09f, 1.00f);
            colors[(int)ImGuiCol.FrameBgHovered]         = new System.Numerics.Vector4(0.26f, 0.26f, 0.26f, 1.00f);
            colors[(int)ImGuiCol.FrameBgActive]          = new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            colors[(int)ImGuiCol.TitleBg]                = new System.Numerics.Vector4(0.09f, 0.09f, 0.09f, 1.00f);
            colors[(int)ImGuiCol.TitleBgActive]          = new System.Numerics.Vector4(0.17f, 0.17f, 0.17f, 1.00f);
            colors[(int)ImGuiCol.TitleBgCollapsed]       = new System.Numerics.Vector4(0.09f, 0.09f, 0.09f, 0.75f);
            colors[(int)ImGuiCol.MenuBarBg]              = new System.Numerics.Vector4(0.09f, 0.09f, 0.09f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarBg]            = new System.Numerics.Vector4(0.09f, 0.09f, 0.09f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrab]          = new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.31f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered]   = new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabActive]    = new System.Numerics.Vector4(0.04f, 0.04f, 0.04f, 1.00f);
            colors[(int)ImGuiCol.CheckMark]              = new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.31f);
            colors[(int)ImGuiCol.SliderGrab]             = new System.Numerics.Vector4(0.80f, 0.80f, 0.83f, 0.31f);
            colors[(int)ImGuiCol.SliderGrabActive]       = new System.Numerics.Vector4(0.04f, 0.04f, 0.04f, 1.00f);
            colors[(int)ImGuiCol.Button]                 = new System.Numerics.Vector4(0.09f, 0.09f, 0.09f, 0.91f);
            colors[(int)ImGuiCol.ButtonHovered]          = new System.Numerics.Vector4(0.26f, 0.26f, 0.26f, 1.00f);
            colors[(int)ImGuiCol.ButtonActive]           = new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            colors[(int)ImGuiCol.Header]                 = new System.Numerics.Vector4(0.09f, 0.09f, 0.09f, 1.00f);
            colors[(int)ImGuiCol.HeaderHovered]          = new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            colors[(int)ImGuiCol.HeaderActive]           = new System.Numerics.Vector4(0.04f, 0.04f, 0.04f, 1.00f);
            colors[(int)ImGuiCol.Separator]              = new System.Numerics.Vector4(0.43f, 0.43f, 0.50f, 0.50f);
            colors[(int)ImGuiCol.SeparatorHovered]       = new System.Numerics.Vector4(0.10f, 0.40f, 0.75f, 0.78f);
            colors[(int)ImGuiCol.SeparatorActive]        = new System.Numerics.Vector4(0.10f, 0.40f, 0.75f, 1.00f);
            colors[(int)ImGuiCol.ResizeGrip]             = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.ResizeGripHovered]      = new System.Numerics.Vector4(0.56f, 0.56f, 0.58f, 1.00f);
            colors[(int)ImGuiCol.ResizeGripActive]       = new System.Numerics.Vector4(0.04f, 0.04f, 0.04f, 1.00f);
            colors[(int)ImGuiCol.Tab]                    = new System.Numerics.Vector4(0.17f, 0.17f, 0.17f, 0.86f);
            colors[(int)ImGuiCol.TabHovered]             = new System.Numerics.Vector4(0.26f, 0.26f, 0.26f, 0.80f);
            colors[(int)ImGuiCol.TabActive]              = new System.Numerics.Vector4(0.39f, 0.39f, 0.39f, 1.00f);
            colors[(int)ImGuiCol.TabUnfocused]           = new System.Numerics.Vector4(0.09f, 0.09f, 0.09f, 0.97f);
            colors[(int)ImGuiCol.TabUnfocusedActive]     = new System.Numerics.Vector4(0.13f, 0.13f, 0.13f, 1.00f);
            colors[(int)ImGuiCol.PlotLines]              = new System.Numerics.Vector4(0.40f, 0.39f, 0.38f, 0.63f);
            colors[(int)ImGuiCol.PlotLinesHovered]       = new System.Numerics.Vector4(0.25f, 1.00f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogram]          = new System.Numerics.Vector4(0.40f, 0.39f, 0.38f, 0.63f);
            colors[(int)ImGuiCol.PlotHistogramHovered]   = new System.Numerics.Vector4(0.25f, 1.00f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TextSelectedBg]         = new System.Numerics.Vector4(0.25f, 1.00f, 0.00f, 0.43f);
            colors[(int)ImGuiCol.DragDropTarget]         = new System.Numerics.Vector4(1.00f, 1.00f, 0.00f, 0.90f);
            colors[(int)ImGuiCol.NavHighlight]           = new System.Numerics.Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            colors[(int)ImGuiCol.NavWindowingHighlight]  = new System.Numerics.Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            colors[(int)ImGuiCol.NavWindowingDimBg]      = new System.Numerics.Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            colors[(int)ImGuiCol.ModalWindowDimBg]       = new System.Numerics.Vector4(1.00f, 0.98f, 0.95f, 0.73f);

            var style = ImGui.GetStyle();
	        style.WindowPadding = new System.Numerics.Vector2(16, 16);
	        style.WindowRounding = 5.0f;
	        style.FramePadding = new System.Numerics.Vector2(6, 6);
	        style.FrameRounding = 4.0f;
	        style.ItemSpacing = new System.Numerics.Vector2(12, 8);
	        style.ItemInnerSpacing = new System.Numerics.Vector2(8, 6);
	        style.IndentSpacing = 25.0f;
	        style.ScrollbarSize = 15.0f;
	        style.ScrollbarRounding = 9.0f;
	        style.GrabMinSize = 5.0f;
	        style.GrabRounding = 3.0f;
        }

        private void SetupVguiStyle()
        {
            // TODO: load from a file in future
            // VGUI style taken from https://github.com/ocornut/imgui/issues/707
            var colors = ImGui.GetStyle().Colors;
            colors[(int)ImGuiCol.Text]                       = new System.Numerics.Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled]               = new System.Numerics.Vector4(0.50f, 0.50f, 0.50f, 1.00f);
            colors[(int)ImGuiCol.WindowBg]                   = new System.Numerics.Vector4(0.29f, 0.34f, 0.26f, 1.00f);
            colors[(int)ImGuiCol.ChildBg]                    = new System.Numerics.Vector4(0.29f, 0.34f, 0.26f, 1.00f);
            colors[(int)ImGuiCol.PopupBg]                    = new System.Numerics.Vector4(0.24f, 0.27f, 0.20f, 1.00f);
            colors[(int)ImGuiCol.Border]                     = new System.Numerics.Vector4(0.54f, 0.57f, 0.51f, 0.50f);
            colors[(int)ImGuiCol.BorderShadow]               = new System.Numerics.Vector4(0.14f, 0.16f, 0.11f, 0.52f);
            colors[(int)ImGuiCol.FrameBg]                    = new System.Numerics.Vector4(0.24f, 0.27f, 0.20f, 1.00f);
            colors[(int)ImGuiCol.FrameBgHovered]             = new System.Numerics.Vector4(0.27f, 0.30f, 0.23f, 1.00f);
            colors[(int)ImGuiCol.FrameBgActive]              = new System.Numerics.Vector4(0.30f, 0.34f, 0.26f, 1.00f);
            colors[(int)ImGuiCol.TitleBg]                    = new System.Numerics.Vector4(0.24f, 0.27f, 0.20f, 1.00f);
            colors[(int)ImGuiCol.TitleBgActive]              = new System.Numerics.Vector4(0.29f, 0.34f, 0.26f, 1.00f);
            colors[(int)ImGuiCol.TitleBgCollapsed]           = new System.Numerics.Vector4(0.00f, 0.00f, 0.00f, 0.51f);
            colors[(int)ImGuiCol.MenuBarBg]                  = new System.Numerics.Vector4(0.24f, 0.27f, 0.20f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarBg]                = new System.Numerics.Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrab]              = new System.Numerics.Vector4(0.28f, 0.32f, 0.24f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered]       = new System.Numerics.Vector4(0.25f, 0.30f, 0.22f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabActive]        = new System.Numerics.Vector4(0.23f, 0.27f, 0.21f, 1.00f);
            colors[(int)ImGuiCol.CheckMark]                  = new System.Numerics.Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            colors[(int)ImGuiCol.SliderGrab]                 = new System.Numerics.Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            colors[(int)ImGuiCol.SliderGrabActive]           = new System.Numerics.Vector4(0.54f, 0.57f, 0.51f, 0.50f);
            colors[(int)ImGuiCol.Button]                     = new System.Numerics.Vector4(0.29f, 0.34f, 0.26f, 0.40f);
            colors[(int)ImGuiCol.ButtonHovered]              = new System.Numerics.Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            colors[(int)ImGuiCol.ButtonActive]               = new System.Numerics.Vector4(0.54f, 0.57f, 0.51f, 0.50f);
            colors[(int)ImGuiCol.Header]                     = new System.Numerics.Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            colors[(int)ImGuiCol.HeaderHovered]              = new System.Numerics.Vector4(0.35f, 0.42f, 0.31f, 0.6f);
            colors[(int)ImGuiCol.HeaderActive]               = new System.Numerics.Vector4(0.54f, 0.57f, 0.51f, 0.50f);
            colors[(int)ImGuiCol.Separator]                  = new System.Numerics.Vector4(0.14f, 0.16f, 0.11f, 1.00f);
            colors[(int)ImGuiCol.SeparatorHovered]           = new System.Numerics.Vector4(0.54f, 0.57f, 0.51f, 1.00f);
            colors[(int)ImGuiCol.SeparatorActive]            = new System.Numerics.Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            colors[(int)ImGuiCol.ResizeGrip]                 = new System.Numerics.Vector4(0.19f, 0.23f, 0.18f, 0.00f); // grip invis
            colors[(int)ImGuiCol.ResizeGripHovered]          = new System.Numerics.Vector4(0.54f, 0.57f, 0.51f, 1.00f);
            colors[(int)ImGuiCol.ResizeGripActive]           = new System.Numerics.Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            colors[(int)ImGuiCol.Tab]                        = new System.Numerics.Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            colors[(int)ImGuiCol.TabHovered]                 = new System.Numerics.Vector4(0.54f, 0.57f, 0.51f, 0.78f);
            colors[(int)ImGuiCol.TabActive]                  = new System.Numerics.Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            colors[(int)ImGuiCol.TabUnfocused]               = new System.Numerics.Vector4(0.24f, 0.27f, 0.20f, 1.00f);
            colors[(int)ImGuiCol.TabUnfocusedActive]         = new System.Numerics.Vector4(0.35f, 0.42f, 0.31f, 1.00f);
            // colors[(int)ImGuiCol.DockingPreview]             = new System.Numerics.Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            // colors[(int)ImGuiCol.DockingEmptyBg]             = new System.Numerics.Vector4(0.20f, 0.20f, 0.20f, 1.00f);
            colors[(int)ImGuiCol.PlotLines]                  = new System.Numerics.Vector4(0.61f, 0.61f, 0.61f, 1.00f);
            colors[(int)ImGuiCol.PlotLinesHovered]           = new System.Numerics.Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogram]              = new System.Numerics.Vector4(1.00f, 0.78f, 0.28f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogramHovered]       = new System.Numerics.Vector4(1.00f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TextSelectedBg]             = new System.Numerics.Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            colors[(int)ImGuiCol.DragDropTarget]             = new System.Numerics.Vector4(0.73f, 0.67f, 0.24f, 1.00f);
            colors[(int)ImGuiCol.NavHighlight]               = new System.Numerics.Vector4(0.59f, 0.54f, 0.18f, 1.00f);
            colors[(int)ImGuiCol.NavWindowingHighlight]      = new System.Numerics.Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            colors[(int)ImGuiCol.NavWindowingDimBg]          = new System.Numerics.Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            colors[(int)ImGuiCol.ModalWindowDimBg]           = new System.Numerics.Vector4(0.80f, 0.80f, 0.80f, 0.35f);

            var style = ImGui.GetStyle();
            style.FrameBorderSize = 1.0f;
            style.WindowRounding = 0.0f;
            style.ChildRounding = 0.0f;
            style.FrameRounding = 0.0f;
            style.PopupRounding = 0.0f;
            style.ScrollbarRounding = 0.0f;
            style.GrabRounding = 0.0f;
            style.TabRounding = 0.0f;

            // ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 3f);
        }

        #region "Initialization"
        private void InitGl()
        {
            shaderComponent = new ShaderComponent(new Shader("Content/Shaders/ImGUI/imgui.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/ImGUI/imgui.vert", Shader.Type.VertexShader));

            vao = Gl.GenVertexArray();
            vbo = Gl.GenBuffer();
            ebo = Gl.GenBuffer();
        }

        private void InitFonts()
        {
            var glyphMinAdvanceX = 24;
            var fontSizePixels = ptToPx * 12;

            // io.Fonts.AddFontDefault();
            unsafe
            {
                var stdConfig = ImGuiNative.ImFontConfig_ImFontConfig();

                io.Fonts.AddFontFromFileTTF("Content/Fonts/OpenSans/OpenSans-SemiBold.ttf", fontSizePixels, stdConfig);

                ImGuiNative.ImFontConfig_destroy(stdConfig);

                var faRanges = new ushort[] { FontAwesome5.IconMin, FontAwesome5.IconMax, 0 };
                var faConfig = ImGuiNative.ImFontConfig_ImFontConfig();
                faConfig->MergeMode = 1;
                faConfig->GlyphMinAdvanceX = glyphMinAdvanceX;

                fixed (ushort* rangePtr = faRanges)
                {
                    io.Fonts.AddFontFromFileTTF("Content/Fonts/FontAwesome/fa-solid-900.ttf", fontSizePixels, faConfig, (IntPtr)rangePtr);
                }

                ImGuiNative.ImFontConfig_destroy(faConfig);
            }
            
            io.Fonts.Build();

            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out var width, out var height, out var bpp);
            io.Fonts.SetTexID((IntPtr)1);
            defaultFontTexture = new Texture2D(pixels, width, height, bpp);
            io.Fonts.ClearTexData();
        }

        private void InitKeymap()
        {
            io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyCode.Tab;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyCode.Return;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyCode.Back;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyCode.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyCode.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyCode.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyCode.Down;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyCode.Delete;
            io.KeyMap[(int)ImGuiKey.Space] = (int)KeyCode.Space;
            io.KeyMap[(int)ImGuiKey.Home] = (int)KeyCode.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)KeyCode.End;

            // Below are for copy, cut, paste, undo, select all, etc.
            io.KeyMap[(int)ImGuiKey.C] = (int)KeyCode.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)KeyCode.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)KeyCode.X;
            io.KeyMap[(int)ImGuiKey.Z] = (int)KeyCode.Z;
            io.KeyMap[(int)ImGuiKey.A] = (int)KeyCode.A;
        }

        #endregion

        #region "GUI Elements"

        private void DrawMenuBar()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(4, 6));
            ImGui.BeginMainMenuBar();

            foreach (var menu in Menus)
            {
                if (menu.windows.Count < 0) continue;

                if (ImGui.BeginMenu($"{menu.IconGlyph} {menu.Title}"))
                {
                    foreach (var window in menu.windows)
                    {
                        if (ImGui.MenuItem($"{window.IconGlyph} {window.Title}"))
                        {
                            window.Render = !window.Render;
                        }
                    }

                    ImGui.EndMenu();
                }
            }

            ImGui.EndMainMenuBar();
            ImGui.PopStyleVar();
        }
        #endregion

        public override void Run()
        {
#if DEBUG
            ImGui.NewFrame();

            if (showEditor)
            {
                DrawMenuBar();

                foreach (var menu in Menus)
                {
                    foreach (var window in menu.windows)
                    {
                        if (window.Render)
                        {
                            ImGui.Begin($"{window.IconGlyph} {window.Title}");
                            window.Draw();
                            ImGui.End();
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Overlays.Count; ++i)
                {
                    var window = Overlays[i];
                    if (window != null && window.Render)
                        window.Draw();
                }
            }

            ImGui.Render();
            RenderImGui(ImGui.GetDrawData());
#endif
        }

        private void RenderImGui(ImDrawDataPtr drawData)
        {
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.Disable(EnableCap.CullFace);
            Gl.Disable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.ScissorTest);

            io.DisplaySize = new Vector2(GameSettings.GameResolutionX, GameSettings.GameResolutionY);
            var projectionMatrix = Matrix4x4f.Ortho2D(0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f);

            shaderComponent.UseShader();
            shaderComponent.SetVariable("albedoTexture", 0);
            shaderComponent.SetVariable("projection", projectionMatrix);

            Gl.BindVertexArray(vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            Gl.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            Gl.EnableVertexAttribArray(0); // Position
            Gl.EnableVertexAttribArray(1); // UV Coord
            Gl.EnableVertexAttribArray(2); // Colors
            Gl.VertexAttribPointer(0, 2, VertexAttribType.Float, false, 20, (IntPtr)0);
            Gl.VertexAttribPointer(1, 2, VertexAttribType.Float, false, 20, (IntPtr)8);
            Gl.VertexAttribPointer(2, 4, VertexAttribType.UnsignedByte, true, 20, (IntPtr)16);

            var clipOffset = drawData.DisplayPos;
            var clipScale = drawData.FramebufferScale;

            for (var commandListIndex = 0; commandListIndex < drawData.CmdListsCount; commandListIndex++)
            {
                int indexOffset = 0;
                var commandList = drawData.CmdListsRange[commandListIndex];

                Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(commandList.VtxBuffer.Size * 20), commandList.VtxBuffer.Data, BufferUsage.DynamicDraw);
                Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)(commandList.IdxBuffer.Size * 2), commandList.IdxBuffer.Data, BufferUsage.DynamicDraw);

                for (var commandIndex = 0; commandIndex < commandList.CmdBuffer.Size; commandIndex++)
                {
                    var currentCommand = commandList.CmdBuffer[commandIndex];

                    var clipBounds = new Vector4(
                        (currentCommand.ClipRect.X - clipOffset.X) * clipScale.X,
                        (currentCommand.ClipRect.Y - clipOffset.Y) * clipScale.Y,
                        (currentCommand.ClipRect.Z - clipOffset.X) * clipScale.X,
                        (currentCommand.ClipRect.W - clipOffset.Y) * clipScale.Y
                        );
                    Gl.Scissor((int)clipBounds.x, (int)(windowSize.Y - clipBounds.w), (int)(clipBounds.z - clipBounds.x), (int)(clipBounds.w - clipBounds.y));
                    defaultFontTexture.Bind();
                    Gl.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)currentCommand.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(indexOffset * 2), 0);

                    indexOffset += (int)currentCommand.ElemCount;
                }
            }

            // TODO: Move these elsewhere to prevent later confusion
            Gl.Disable(EnableCap.ScissorTest);
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.CullFace);
        }

        private char KeyCodeToChar(KeyCode keyCode)
        {
            // TODO: correctly handle different locales and keyboard layouts
            // This is currently only written for ISO UK qwerty.
            var c = '\0';
            if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
            {
                c = (char)('a' + keyCode - KeyCode.A);

                if (io.KeyShift)
                {
                    c = char.ToUpper(c);
                }
            }
            else if (keyCode >= KeyCode.N0 && keyCode <= KeyCode.N9)
            {
                if (io.KeyShift)
                {
                    var characters = new List<char>
                    {
                        // 0   1    2    3    4    5    6    7    8    9   
                        ')', '!', '"', '£', '$', '%', '^', '&', '*', '('
                    };
                    c = characters[keyCode - KeyCode.N0];
                }
                else
                {
                    c = (char)('0' + keyCode - KeyCode.N0);
                }
            }
            else if (keyCode >= KeyCode.Numpad0 && keyCode <= KeyCode.Numpad9)
            {
                c = (char)('0' + keyCode - KeyCode.Numpad0);
            }
            else
            {
                switch (keyCode)
                {
                    case KeyCode.Space:
                        c = ' ';
                        break;
                    case KeyCode.Tab:
                        c = '\t';
                        break;
                    case KeyCode.Decimal:
                    case KeyCode.Period:
                        c = '.';
                        break;
                    case KeyCode.Comma:
                        c = ',';
                        break;
                    case KeyCode.Plus:
                        c = '+';
                        break;
                    case KeyCode.Minus:
                        c = '-';
                        break;
                    case KeyCode.Return:
                    case KeyCode.Right:
                    case KeyCode.Up:
                    case KeyCode.Down:
                    case KeyCode.Left:
                    case KeyCode.Delete:
                    case KeyCode.Back:
                    case KeyCode.Shift:
                    case KeyCode.Control:
                        // Ignore
                        break;
                    default:
                        c = '?';
                        Logging.Log($"Unhandled character {keyCode} received");
                        break;
                }
            }
            return c;
        }

        public override void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            switch (eventType)
            {
                case Event.KeyDown:
                    {
                        var keyboardEventArgs = (KeyboardEventArgs)eventArgs;
                        io.KeysDown[keyboardEventArgs.KeyboardKey] = true;
                        var keyCode = (KeyCode)keyboardEventArgs.KeyboardKey;

                        switch (keyCode)
                        {
                            case KeyCode.Shift:
                                io.KeyShift = true;
                                break;
                            case KeyCode.Control:
                                io.KeyCtrl = true;
                                break;
                            case KeyCode.F1:
                                showEditor = !showEditor;
                                break;
                            case KeyCode.F2:
                                lockCursor = !lockCursor;
                                break;
                            case KeyCode.F3:
                                // See ConsoleWindow.cs
                                break;
                            default:
                                io.AddInputCharacter(KeyCodeToChar(keyCode));
                                break;
                        }
                        break;
                    }
                case Event.KeyUp:
                    {
                        var keyboardEventArgs = (KeyboardEventArgs)eventArgs;
                        io.KeysDown[keyboardEventArgs.KeyboardKey] = false;
                        var keyCode = (KeyCode)keyboardEventArgs.KeyboardKey;
                        if (keyCode == KeyCode.Shift)
                        {
                            io.KeyShift = false;
                        }
                        else if (keyCode == KeyCode.Control)
                        {
                            io.KeyCtrl = false;
                        }
                        break;
                    }
                case Event.MouseMove:
                    {
                        var mouseMoveEventArgs = (MouseMoveEventArgs)eventArgs;
                        io.MousePos = new Vector2(mouseMoveEventArgs.MousePosition.x, mouseMoveEventArgs.MousePosition.y);
                        break;
                    }
                case Event.MouseButtonDown:
                    {
                        var mouseButtonEventArgs = (MouseButtonEventArgs)eventArgs;
                        io.MouseDown[mouseButtonEventArgs.MouseButton] = true;
                        break;
                    }
                case Event.MouseButtonUp:
                    {
                        var mouseButtonEventArgs = (MouseButtonEventArgs)eventArgs;
                        io.MouseDown[mouseButtonEventArgs.MouseButton] = false;
                        break;
                    }
                case Event.MouseScroll:
                    {
                        var mouseWheelEventArgs = (MouseWheelEventArgs)eventArgs;
                        io.MouseWheel += mouseWheelEventArgs.MouseScroll;
                        break;
                    }
                case Event.WindowResized:
                    {
                        var windowResizeEventArgs = (WindowResizeEventArgs)eventArgs;
                        windowSize = new Vector2(windowResizeEventArgs.WindowSize.x, windowResizeEventArgs.WindowSize.y);
                        break;
                    }
                default:
                    Logging.Log($"Unhandled event of type {eventType}");
                    break;
            }
        }
    }
}
