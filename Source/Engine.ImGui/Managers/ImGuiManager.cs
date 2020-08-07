﻿using Engine.Assets;
using Engine.ECS.Observer;
using Engine.ECS.Managers;
using Engine.Gui.Managers.ImGuiWindows;
using Engine.Gui.Managers.ImGuiWindows.Editor;
using Engine.Gui.Managers.ImGuiWindows.Overlays;
using Engine.Gui.Managers.ImGuiWindows.Theming;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Render;
using Engine.Utils;
using Engine.Utils.DebugUtils;
using ImGuiNET;
using OpenGL;
using OpenGL.CoreUI;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vector4f = Engine.Utils.MathUtils.Vector4f;

namespace Engine.Gui.Managers
{
    public class ImGuiManager : Manager<ImGuiManager>
    {
        private const float PT_TO_PX = 1.3281472327365f;

        private Texture2D defaultFontTexture;
        private ShaderComponent shaderComponent;
        private Vector2 windowSize;

        private readonly ImGuiIOPtr io;

        private uint vbo, vao, ebo;
        private bool showEditor;
        private bool lockCursor;

        private ImGuiTheme theme;
        public ImGuiTheme Theme
        {
            get => theme;
            set
            {
                value.SetTheme();
                theme = value;
            }
        }

        public List<ImGuiMenu> Menus { get; } = new List<ImGuiMenu>()
        {
            new ImGuiMenu(FontAwesome5.File, "File", new List<ImGuiWindow>()
            {
                new SaveSettingsWindow(),
                new CloseGameWindow()
            }),
            //new ImGuiMenu(FontAwesome5.FileCode, "Scripts", new List<ImGuiWindow>()
            //{
            //    new ScriptCompileWindow()
            //}),
            new ImGuiMenu(FontAwesome5.Cubes, "Scene", new List<ImGuiWindow>()
            {
                new ViewportWindow(),
                new ScenePropertiesWindow()
            }),
            new ImGuiMenu(FontAwesome5.Wrench, "Engine", new List<ImGuiWindow>()
            {
                new EngineConfigWindow(),
                new PerformanceWindow(),
                new TextureBrowserWindow(),
                new ShaderWindow(),
                new InputWindow()
            })
        };

        public List<ImGuiWindow> Overlays { get; } = new List<ImGuiWindow>()
        {
            new UnfocusedConsoleWindow(),
            new FocusedConsoleWindow(),
            new PerformanceOverlayWindow()
        };
        public ImFontPtr MonospacedFont { get; private set; }

        public ImGuiManager()
        {
            var imGuiContext = ImGui.CreateContext();

            if (ImGui.GetCurrentContext() == IntPtr.Zero)
                ImGui.SetCurrentContext(imGuiContext);

            io = ImGui.GetIO();

            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

            ImGui.StyleColorsDark();

            // Set default theme from game settings
            Theme = ImGuiTheme.LoadFromFile($"Content/Themes/{GameSettings.EditorTheme}.json");
            // TODO: Check if theme doesn't exist, set a default
            // TODO: Move code to somewhere that makes more sense?

            InitFonts();
            InitKeymap();
            InitGl();
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
            var fontSizePixels = PT_TO_PX * 12;

            // Standard fonts
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

            // Monospaced fonts
            unsafe
            {
                var stdConfig = ImGuiNative.ImFontConfig_ImFontConfig();

                MonospacedFont = io.Fonts.AddFontFromFileTTF("Content/Fonts/IBMPlexMono/IBMPlexMono-SemiBold.ttf", fontSizePixels, stdConfig).NativePtr;

                ImGuiNative.ImFontConfig_destroy(stdConfig);
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
                            ImGui.Begin($"{window.IconGlyph} {window.Title}", window.Flags);
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
            // TODO: use abstract graphics impl
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

                    var clipBounds = new Vector4f(
                            (currentCommand.ClipRect.X - clipOffset.X) * clipScale.X,
                            (currentCommand.ClipRect.Y - clipOffset.Y) * clipScale.Y,
                            (currentCommand.ClipRect.Z - clipOffset.X) * clipScale.X,
                            (currentCommand.ClipRect.W - clipOffset.Y) * clipScale.Y
                        );
                    Gl.Scissor((int)clipBounds.x, (int)(windowSize.Y - clipBounds.w), (int)(clipBounds.z - clipBounds.x), (int)(clipBounds.w - clipBounds.y));

                    if ((uint)currentCommand.TextureId == 1)
                        defaultFontTexture.Bind();
                    else
                        Gl.BindTexture(TextureTarget.Texture2d, (uint)currentCommand.TextureId);
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

            // TODO: Handle modifier keys
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
                    case KeyCode.OEM3:
                        c = '@';
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

        public override void OnNotify(NotifyType eventType, INotifyArgs notifyArgs)
        {
            switch (eventType)
            {
                case NotifyType.KeyDown:
                    {
                        var keyboardEventArgs = (KeyboardNotifyArgs)notifyArgs;
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
                case NotifyType.KeyUp:
                    {
                        var keyboardEventArgs = (KeyboardNotifyArgs)notifyArgs;
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
                case NotifyType.MouseMove:
                    {
                        var mouseMoveEventArgs = (MouseMoveNotifyArgs)notifyArgs;
                        io.MousePos = new Vector2(mouseMoveEventArgs.MousePosition.x, mouseMoveEventArgs.MousePosition.y);
                        break;
                    }
                case NotifyType.MouseButtonDown:
                    {
                        var mouseButtonEventArgs = (MouseButtonNotifyArgs)notifyArgs;
                        io.MouseDown[mouseButtonEventArgs.MouseButton] = true;
                        break;
                    }
                case NotifyType.MouseButtonUp:
                    {
                        var mouseButtonEventArgs = (MouseButtonNotifyArgs)notifyArgs;
                        io.MouseDown[mouseButtonEventArgs.MouseButton] = false;
                        break;
                    }
                case NotifyType.MouseScroll:
                    {
                        var mouseWheelEventArgs = (MouseWheelNotifyArgs)notifyArgs;
                        io.MouseWheel += mouseWheelEventArgs.MouseScroll;
                        break;
                    }
                case NotifyType.WindowResized:
                    {
                        var windowResizeEventArgs = (WindowResizeNotifyArgs)notifyArgs;
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
