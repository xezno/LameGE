using System;
using System.Collections.Generic;
using System.Numerics;
using ECSEngine.Components;
using ECSEngine.Entities;
using ECSEngine.Events;
using ECSEngine.Render;
using ImGuiNET;
using OpenGL;
using OpenGL.CoreUI;
using Vector4 = ECSEngine.MathUtils.Vector4;

namespace ECSEngine.Managers
{
    public class ImGuiManager : Manager<ImGuiManager>
    {
        private Texture2D defaultFontTexture;
        private ShaderComponent shaderComponent;
        private Vector2 windowSize;
        private ImGuiIOPtr io;
        private IEntity selectedEntity;

        private uint vbo, vao, ebo;
        private int currentSceneHierarchyItem;
        private string currentConsoleInput = "", currentConsoleFilter = "";
        private bool shouldRender, showPlayground, showSceneHierarchy, showPerformanceStats, showInspector, showConsole, showConsolePreview = true;

        public ImGuiManager()
        {
            var imGuiContext = ImGui.CreateContext();
            if (ImGui.GetCurrentContext() == IntPtr.Zero)
                ImGui.SetCurrentContext(imGuiContext);
            io = ImGui.GetIO();

            io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.MouseDrawCursor = true; // Use custom mouse cursor

            ImGui.StyleColorsDark();

            InitFonts();
            InitKeymap();
            InitGl();
        }

        #region "Initialization"
        private void InitGl()
        {
            shaderComponent = new ShaderComponent(new Shader("Content/ImGUI/imgui.frag", ShaderType.FragmentShader),
                new Shader("Content/ImGUI/imgui.vert", ShaderType.VertexShader));

            vao = Gl.GenVertexArray();
            vbo = Gl.GenBuffer();
            ebo = Gl.GenBuffer();
        }

        private void InitFonts()
        {
            io.Fonts.AddFontFromFileTTF("Content/Fonts/OpenSans/OpenSans-SemiBold.ttf", 16);
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
            ImGui.BeginMainMenuBar();
            if (ImGui.BeginMenu("Window"))
            {
                if (ImGui.MenuItem("Toggle scene hierarchy"))
                    showSceneHierarchy = !showSceneHierarchy;
                if (ImGui.MenuItem("Toggle inspector"))
                    showInspector = !showInspector;
                if (ImGui.MenuItem("Toggle performance stats"))
                    showPerformanceStats = !showPerformanceStats;
                if (ImGui.MenuItem("Toggle playground"))
                    showPlayground = !showPlayground;
                if (ImGui.MenuItem("Toggle console (`)"))
                    showConsole = !showConsole;

                if (ImGui.MenuItem("Show all"))
                {
                    showSceneHierarchy = true;
                    showInspector = true;
                    showPerformanceStats = true;
                    showPlayground = true;
                    showConsole = true;
                }
                if (ImGui.MenuItem("Hide all"))
                {
                    showSceneHierarchy = false;
                    showInspector = false;
                    showPerformanceStats = false;
                    showPlayground = false;
                    showConsole = false;
                }

                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }

        private void DrawPlayground()
        {
            ImGui.Begin("Playground", ref showPlayground);

            // Loading test
            var progress = (int)(ImGui.GetTime() / 0.025f) % 100;
            var filesLoaded = (int)Math.Round(progress * 0.25);
            ImGui.Text($"Loading, please wait... {@"|/-\"[(int)(ImGui.GetTime() / 0.25f) % 4]}");
            ImGui.TextUnformatted($"Files loaded: {progress}% ({filesLoaded} / 25)"); // TextUnformatted displays '%' without needing to format it as '%%'.

            ImGui.End();
        }

        private void DrawInspector()
        {
            ImGui.Begin("Inspector Gadget", ref showInspector);

            if (selectedEntity == null)
            {
                ImGui.End();
                return;
            }

            selectedEntity.RenderImGUI();

            ImGui.End();
        }

        private void DrawSceneHierarchy()
        {
            ImGui.Begin("Scene Hierarchy", ref showSceneHierarchy);
            var entityNames = new string[SceneManager.Instance.Entities.Count];
            for (var i = 0; i < SceneManager.Instance.Entities.Count; i++)
            {
                entityNames[i] = SceneManager.Instance.Entities[i].GetType().Name;
            }

            ImGui.PushItemWidth(-1);

            if (ImGui.ListBox("##SceneListbox", ref currentSceneHierarchyItem, entityNames, entityNames.Length))
            {
                selectedEntity = SceneManager.Instance.Entities[currentSceneHierarchyItem];
            }

            ImGui.PopItemWidth();

            ImGui.End();
        }

        private void DrawPerformanceStats()
        {
            ImGui.Begin("Performance", ref showPerformanceStats);

            ImGui.LabelText($"{RenderManager.Instance.LastFrameTime}ms", "Current frametime");

            ImGui.PlotHistogram(
                "Average frame time",
                ref RenderManager.Instance.FrametimeHistory[0],
                RenderManager.Instance.FrametimeHistory.Length,
                0,
                "",
                0
            );

            ImGui.LabelText($"{RenderManager.Instance.CalculatedFramerate}fps", "Current framerate");
            ImGui.PlotLines(
                "Average frame rate",
                ref RenderManager.Instance.FramerateHistory[0],
                RenderManager.Instance.FramerateHistory.Length,
                0,
                "",
                0
            );

            ImGui.Checkbox("Fake lag", ref RenderManager.Instance.fakeLag);

            ImGui.End();
        }

        private void DrawConsole()
        {
            ImGui.Begin("Console", ref showConsole);

            ImGui.PushItemWidth(-1);
            ImGui.InputTextMultiline("Console", ref Debug.pastLogsStringConsole, UInt32.MaxValue, new Vector2(-1, -50), ImGuiInputTextFlags.ReadOnly);
            ImGui.SetScrollHereY(1.0f);
            ImGui.PopItemWidth();

            if (ImGui.InputText("Filter", ref currentConsoleFilter, 256))
            {
                Debug.CalcLogStringByFilter(currentConsoleFilter);
            }

            ImGui.InputText("Input", ref currentConsoleInput, 256);

            ImGui.End();
        }
        #endregion

        public override void Run()
        {
#if DEBUG
            ImGui.NewFrame();
            if (shouldRender)
            {
                DrawMenuBar();

                if (showPlayground)
                    DrawPlayground();

                if (showSceneHierarchy)
                    DrawSceneHierarchy();

                if (showPerformanceStats)
                    DrawPerformanceStats();

                if (showInspector)
                    DrawInspector();

                if (showConsole)
                    DrawConsole();
            }
            else
            {
                var debugText = "ECS Engine\n" +
                                   "F1 for editor\n" +
                                   $"{RenderManager.Instance.LastFrameTime}ms\n" +
                                   $"{RenderManager.Instance.CalculatedFramerate}fps";
                var debugTextPos = new Vector2(RenderSettings.Default.gameResolutionX - 128, 8);
                ImGui.GetBackgroundDrawList().AddText(
                    debugTextPos + new Vector2(1, 1), 0x88000000, debugText); // Shadow
                ImGui.GetBackgroundDrawList().AddText(
                    debugTextPos, 0xFFFFFFFF, debugText);
            }

            if (showConsolePreview && !(shouldRender && showConsole))
            {
                var consoleText = Debug.PastLogsString;
                var consoleTextPos = new Vector2(8, 8);

                if (shouldRender)
                    consoleTextPos.Y += 20; // Offset if menu bar is visible

                ImGui.GetBackgroundDrawList().AddText(
                    consoleTextPos + new Vector2(1, 1), 0x88000000, consoleText); // Shadow
                ImGui.GetBackgroundDrawList().AddText(
                    consoleTextPos, 0xFFFFFFFF, consoleText);
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

            io.DisplaySize = windowSize;

            for (var commandListIndex = 0; commandListIndex < drawData.CmdListsCount; commandListIndex++)
            {
                int indexOffset = 0, vertexOffset = 0;
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
                    Gl.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)currentCommand.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(indexOffset * 2), vertexOffset);

                    indexOffset += (int)currentCommand.ElemCount;
                }
                vertexOffset += commandList.VtxBuffer.Size;
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
                        Debug.Log($"Unhandled character {keyCode} received");
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
                                shouldRender = !shouldRender;
                                break;
                            case KeyCode.OEM3:
                                showConsole = !showConsole;
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
                    Debug.Log($"Unhandled event of type {eventType}");
                    break;
            }
        }
    }
}
