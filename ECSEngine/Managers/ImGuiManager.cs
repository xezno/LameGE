using ECSEngine.Attributes;
using ECSEngine.Components;
using ECSEngine.Entities;
using ECSEngine.Events;
using ECSEngine.Render;
using ImGuiNET;
using OpenGL;
using OpenGL.CoreUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Quaternion = ECSEngine.MathUtils.Quaternion;
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
        private int currentShaderItem, currentSceneHierarchyItem;
        private string currentShaderSource = "";
        private bool shouldRender, showPlayground, showSceneHierarchy, showPerformanceStats, showInspector, showShaderEditor;

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
            InitGL();
        }

        #region "Initialization"
        private void InitGL()
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
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bpp);
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
                if (ImGui.MenuItem("Toggle shader editor"))
                    showShaderEditor = !showShaderEditor;

                if (ImGui.MenuItem("Show all"))
                {
                    showSceneHierarchy = true;
                    showInspector = true;
                    showPerformanceStats = true;
                    showPlayground = true;
                    showShaderEditor = true;
                }
                if (ImGui.MenuItem("Hide all"))
                {
                    showSceneHierarchy = false;
                    showInspector = false;
                    showPerformanceStats = false;
                    showPlayground = false;
                    showShaderEditor = false;
                }

                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }

        private void DrawPlayground()
        {
            ImGui.Begin("Playground", ref showPlayground);

            // Loading test
            int progress = (int)(ImGui.GetTime() / 0.025f) % 100;
            int filesLoaded = (int)Math.Round(progress * 0.25);
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

            RenderObject(selectedEntity);

            foreach (var objectComponent in selectedEntity.components)
            {
                if (ImGui.TreeNode(objectComponent.GetType().Name))
                {
                    RenderObject(objectComponent);
                    ImGui.TreePop();
                }
            }

            ImGui.End();
        }

        private void DrawSceneHierarchy()
        {
            ImGui.Begin("Scene Hierarchy", ref showSceneHierarchy);
            string[] entityNames = new string[SceneManager.instance.entities.Count];
            for (int i = 0; i < SceneManager.instance.entities.Count; i++)
            {
                entityNames[i] = SceneManager.instance.entities[i].GetType().Name;
            }

            if (ImGui.ListBox("", ref currentSceneHierarchyItem, entityNames, entityNames.Length))
            {
                selectedEntity = SceneManager.instance.entities[currentSceneHierarchyItem];
            }

            ImGui.End();
        }

        private void DrawPerformanceStats()
        {
            ImGui.Begin("Performance", ref showPerformanceStats);

            ImGui.LabelText($"{RenderManager.instance.lastFrameTime}ms", "Current frametime");

            ImGui.PlotHistogram(
                "Average frame time",
                ref RenderManager.instance.frametimeHistory[0],
                RenderManager.instance.frametimeHistory.Length,
                0,
                "",
                0
            );

            ImGui.LabelText($"{RenderManager.instance.calculatedFramerate}fps", "Current framerate");
            ImGui.PlotLines(
                "Average frame rate",
                ref RenderManager.instance.framerateHistory[0],
                RenderManager.instance.framerateHistory.Length,
                0,
                "",
                0
            );

            ImGui.Checkbox($"Fake lag", ref RenderManager.instance.fakeLag);

            ImGui.End();
        }

        private void DrawShaderEditor()
        {
            ImGui.Begin("Shader options", ref showShaderEditor);
            
            if (selectedEntity != null && selectedEntity.HasComponent<ShaderComponent>())
            {
                var selectedShaderComponent = selectedEntity.GetComponent<ShaderComponent>();
                string[] shaderNames = new string[selectedShaderComponent.shaders.Length];
                for (int i = 0; i < selectedShaderComponent.shaders.Length; ++i)
                {
                    shaderNames[i] = selectedShaderComponent.shaders[i].fileName;
                }

                if (selectedShaderComponent.shaders.Length > 0)
                {
                    if (ImGui.ListBox("", ref currentShaderItem, shaderNames, selectedShaderComponent.shaders.Length))
                    {
                        selectedEntity = SceneManager.instance.entities[currentSceneHierarchyItem];
                        currentShaderSource = selectedShaderComponent.shaders[currentShaderItem].shaderSource[0];
                    }

                    if (ImGui.Button("Reload shader"))
                    {
                        selectedShaderComponent.CreateShader();
                        selectedShaderComponent.shaders[currentShaderItem].ReadSourceFromFile();
                        selectedShaderComponent.shaders[currentShaderItem].Compile();
                        selectedShaderComponent.AttachAndLink();
                    }
                }
            }
            else
            {
                ImGui.LabelText("Shader contents", "Select an entity with a shader component.");
                currentShaderItem = 0;
            }
            ImGui.End();
        }
        #endregion

        public override void Run()
        {
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

                if (showShaderEditor)
                    DrawShaderEditor();
            }
            else
            {
#if DEBUG
                ImGui.GetBackgroundDrawList().AddText(
                    new Vector2(0, 0), 0xFFFFFFFF, $"ECS Engine\n" +
                                                   $"Press F1 to open the editor.\n" +
                                                   $"{RenderManager.instance.lastFrameTime}ms, {RenderManager.instance.calculatedFramerate}fps"
                );
#endif
            }

            ImGui.Render();
            RenderImGui(ImGui.GetDrawData());
        }

        private void RenderObject(object objectToRender, int depth = 0)
        {
            // TODO: refactor this so it doesnt use dynamic
            if (depth > 1) return; // Prevent any dumb stack overflow errors
            foreach (var field in objectToRender.GetType().GetFields())
            {
                // this works but is, like, the worst solution ever
                var referenceType = typeof(Ref<>).MakeGenericType(field.FieldType);
                var reference = (dynamic /* alarm bells right here */)Activator.CreateInstance(referenceType, field, objectToRender);
                if (field.FieldType == typeof(float))
                {
                    float value = reference.value;
                    float min = float.MinValue;
                    float max = float.MaxValue;
                    bool useSlider = false;
                    var fieldAttributes = field.GetCustomAttributes(false);
                    foreach (var attrib in fieldAttributes.Where(o => o.GetType() == typeof(RangeAttribute)))
                    {
                        var rangeAttrib = (RangeAttribute)attrib;
                        min = rangeAttrib.min;
                        max = rangeAttrib.max;
                        useSlider = true;
                    }

                    if (useSlider)
                        ImGui.SliderFloat($"{field.Name}", ref value, min, max);
                    else
                        ImGui.InputFloat($"{field.Name}", ref value);
                    reference.value = value;
                }
                else if (field.FieldType == typeof(ColorRGB24))
                {
                    Vector3 value = new Vector3(reference.value.r / 255f, reference.value.g / 255f, reference.value.b / 255f);
                    ImGui.ColorEdit3($"{field.Name}", ref value);
                    reference.value = new ColorRGB24((byte)(value.X * 255f), (byte)(value.Y * 255f), (byte)(value.Z * 255f));
                }
                else if (field.FieldType == typeof(MathUtils.Vector3))
                {
                    Vector3 value = reference.value.ConvertToNumerics();
                    ImGui.DragFloat3(field.Name, ref value, 0.1f);
                    reference.value = MathUtils.Vector3.ConvertFromNumerics(value);
                }
                else if (field.FieldType == typeof(Quaternion))
                {
                    Vector3 value = reference.value.ToEulerAngles().ConvertToNumerics();
                    ImGui.DragFloat3(field.Name, ref value, 0.1f);
                    reference.value = Quaternion.FromEulerAngles(MathUtils.Vector3.ConvertFromNumerics(value));
                }
                else if (field.FieldType == typeof(int))
                {
                    int value = reference.value;
                    ImGui.DragInt($"{field.Name}", ref value);
                    reference.value = value;
                }
                else if (field.FieldType == typeof(List<>) || field.FieldType.BaseType == typeof(Array))
                {
                    foreach (var element in (dynamic /* uh oh, we're at it again*/)field.GetValue(objectToRender))
                    {
                        RenderObject(element, depth + 1);
                    }
                }
                else
                {
                    ImGui.LabelText($"{field.Name}", $"{field.GetValue(objectToRender)}");
                }
            }
        }

        private void RenderImGui(ImDrawDataPtr drawData)
        {
            Gl.BlendEquation(BlendEquationMode.FuncAdd);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            Gl.Disable(EnableCap.CullFace);
            Gl.Disable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.ScissorTest);

            Matrix4x4f projectionMatrix = Matrix4x4f.Ortho2D(0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f);

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

            for (int commandListIndex = 0; commandListIndex < drawData.CmdListsCount; commandListIndex++)
            {
                int indexOffset = 0, vertexOffset = 0;
                ImDrawListPtr commandList = drawData.CmdListsRange[commandListIndex];

                Gl.BufferData(BufferTarget.ArrayBuffer, (uint)(commandList.VtxBuffer.Size * 20), commandList.VtxBuffer.Data, BufferUsage.DynamicDraw);
                Gl.BufferData(BufferTarget.ElementArrayBuffer, (uint)(commandList.IdxBuffer.Size * 2), commandList.IdxBuffer.Data, BufferUsage.DynamicDraw);

                for (int commandIndex = 0; commandIndex < commandList.CmdBuffer.Size; commandIndex++)
                {
                    var currentCommand = commandList.CmdBuffer[commandIndex];

                    Vector4 clipBounds = new Vector4(
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
            char c = '\0';
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
                    List<char> characters = new List<char>()
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
                        KeyboardEventArgs keyboardEventArgs = (KeyboardEventArgs)eventArgs;
                        io.KeysDown[keyboardEventArgs.keyboardKey] = true;
                        KeyCode keyCode = (KeyCode)keyboardEventArgs.keyboardKey;
                        
                        if (keyCode == KeyCode.Shift)
                        {
                            io.KeyShift = true;
                        }
                        else if (keyCode == KeyCode.Control)
                        {
                            io.KeyCtrl = true;
                        }
                        else if (keyCode == KeyCode.F1)
                        {
                            shouldRender = !shouldRender;
                        }
                        else
                        {
                            io.AddInputCharacter(KeyCodeToChar(keyCode));
                        }
                        break;
                    }
                case Event.KeyUp:
                    {
                        KeyboardEventArgs keyboardEventArgs = (KeyboardEventArgs)eventArgs;
                        io.KeysDown[keyboardEventArgs.keyboardKey] = false;
                        KeyCode keyCode = (KeyCode)keyboardEventArgs.keyboardKey;
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
                        MouseMoveEventArgs mouseMoveEventArgs = (MouseMoveEventArgs)eventArgs;
                        io.MousePos = new Vector2(mouseMoveEventArgs.mousePosition.x, mouseMoveEventArgs.mousePosition.y);
                        break;
                    }
                case Event.MouseButtonDown:
                    {
                        MouseButtonEventArgs mouseButtonEventArgs = (MouseButtonEventArgs)eventArgs;
                        io.MouseDown[mouseButtonEventArgs.mouseButton] = true;
                        break;
                    }
                case Event.MouseButtonUp:
                    {
                        MouseButtonEventArgs mouseButtonEventArgs = (MouseButtonEventArgs)eventArgs;
                        io.MouseDown[mouseButtonEventArgs.mouseButton] = false;
                        break;
                    }
                case Event.MouseScroll:
                    {
                        MouseWheelEventArgs mouseWheelEventArgs = (MouseWheelEventArgs)eventArgs;
                        io.MouseWheel += mouseWheelEventArgs.mouseScroll;
                        break;
                    }
                case Event.WindowResized:
                    {
                        WindowResizeEventArgs windowResizeEventArgs = (WindowResizeEventArgs)eventArgs;
                        windowSize = new Vector2(windowResizeEventArgs.windowSize.x, windowResizeEventArgs.windowSize.y);
                        break;
                    }
                default:
                    Debug.Log($"Unhandled event of type {eventType}");
                    break;
            }
        }
    }
}
