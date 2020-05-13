using CefSharp;
using CefSharp.OffScreen;
using CefSharp.Structs;
using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Entities.CEF;
using Engine.Events;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Render;
using Engine.Utils;
using Engine.Utils.DebugUtils;
using Engine.Utils.MathUtils;
using OpenGL;
using System;
using System.IO;
using System.Reflection;
using Size = System.Drawing.Size;

namespace Engine.Entities
{
    // TODO: Move to component, render to a texture and make a HudEntity instead
    public sealed class CefEntity : Entity<CefEntity>
    {
        // private string cefFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Content/UI/index.html";
        private string cefFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Content/UI/vue-rewrite/menus/dist/index.html";

        public override string IconGlyph { get; } = FontAwesome5.Wrench;
        private bool readyToDraw;
        private ChromiumWebBrowser browser;

        private int mouseX, mouseY;
        private RenderHandler renderHandler;

        public CefEntity()
        {
            AddComponent(new ShaderComponent(new Shader("Content/UI/Shaders/main.frag", Shader.Type.FragmentShader),
                new Shader("Content/UI/Shaders/main.vert", Shader.Type.VertexShader)));
            AddComponent(new TransformComponent(new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
            AddComponent(new MaterialComponent(new Material("Content/UI/plane.mtl")));
            AddComponent(new MeshComponent(Primitives.Plane));
            InitBrowser();
        }

        void InitBrowser()
        {
            // setup cef instance
            var browserSettings = new BrowserSettings
            {
                WindowlessFrameRate = 30, // 30 works best here as it prevents flickering, but this might be system dependant 
            };

            var cefSettings = new CefSettings();
            cefSettings.SetOffScreenRenderingBestPerformanceArgs();
            Cef.Initialize(cefSettings);

            var requestContextSettings = new RequestContextSettings();
            var requestContext = new RequestContext(requestContextSettings);
            browser = new ChromiumWebBrowser(cefFilePath, browserSettings, requestContext);
            browser.Size = new Size(GameSettings.GameResolutionX, GameSettings.GameResolutionY);
            browser.RenderHandler = new RenderHandler(browser);
            browser.BrowserInitialized += (sender, args) => { browser.Load(cefFilePath); };
            browser.LoadError += (sender, args) => Logging.Log($"Browser error {args.ErrorCode}");

            renderHandler = (RenderHandler)browser.RenderHandler;

            byte[] emptyData = new byte[browser.Size.Width * browser.Size.Height * 4];
            GetComponent<MaterialComponent>().materials[0].diffuseTexture =
                new Texture2D(emptyData, browser.Size.Width, browser.Size.Height);

            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                if (args.IsLoading) return;

                browser.LoadingStateChanged -= handler;

                // Logging.Logging.Log($"CEF has finished loading page {cefFilePath}");
                readyToDraw = true;
            };

            browser.LoadingStateChanged += handler;
        }

        public override void Render()
        {
            // render cef offscreen & then blit to screen
            // we need to set up texture on the main therad
            // since that wont happen unless we call it here, we need to
            // declare a bool that allows us to detect when we need to
            // setup the texture.

            if (!readyToDraw) return;

            Gl.Disable(EnableCap.DepthTest);

            // draw to screen
            base.Render();
            SetTextureData();

            Gl.Enable(EnableCap.DepthTest);
        }

        private void SetTextureData()
        {
            if (!renderHandler.NeedsPaint)
                return;

            // TODO: Wait until rendering frame, then set tex data, THEN render (currently can render between setting / unsetting tex data - BAD!)
            Paint(renderHandler.Type, renderHandler.DirtyRect, renderHandler.Buffer, renderHandler.Width, renderHandler.Height);
            renderHandler.NeedsPaint = false;
        }

        private void Paint(PaintElementType type, Rect dirtyRect, IntPtr buffer, int width, int height)
        {
            Gl.BindTexture(TextureTarget.Texture2d, GetComponent<MaterialComponent>().materials[0].diffuseTexture.glTexture);

            // Switched to width/height instead of dirtyRect.width/dirtyRect.height, may not work properly?

            if (dirtyRect.X + width > GameSettings.GameResolutionX || dirtyRect.Y + height > GameSettings.GameResolutionY)
            {
                Gl.BindTexture(TextureTarget.Texture2d, 0);
                return;
            }

            Gl.TexSubImage2D(TextureTarget.Texture2d, 0, dirtyRect.X, dirtyRect.Y, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, buffer);
            // Gl.GenerateMipmap(TextureTarget.Texture2d);

            Gl.BindTexture(TextureTarget.Texture2d, 0);
        }

        public override void HandleEvent(Event eventType, IEventArgs baseEventArgs)
        {
            if (eventType == Event.GameEnd)
            {
                //browser.Dispose();
                //requestContext.Dispose();
            }
            else if (eventType == Event.MouseButtonDown)
            {
                var mouseEventArgs = (MouseButtonEventArgs)baseEventArgs;
                var mouseButtonType = mouseEventArgs.MouseButton == 0 ? MouseButtonType.Left : MouseButtonType.Right;
                var eventFlags = mouseButtonType == MouseButtonType.Left ? CefEventFlags.LeftMouseButton : CefEventFlags.RightMouseButton;
                browser.GetBrowserHost().SendMouseClickEvent(new MouseEvent(mouseX, mouseY, eventFlags), mouseButtonType, false, 0);
            }
            else if (eventType == Event.MouseButtonUp)
            {
                var mouseEventArgs = (MouseButtonEventArgs)baseEventArgs;
                var mouseButtonType = mouseEventArgs.MouseButton == 0 ? MouseButtonType.Left : MouseButtonType.Right;
                var eventFlags = mouseButtonType == MouseButtonType.Left ? CefEventFlags.LeftMouseButton : CefEventFlags.RightMouseButton;
                browser.GetBrowserHost().SendMouseClickEvent(new MouseEvent(mouseX, mouseY, eventFlags), mouseButtonType, true, 0);
            }
            else if (eventType == Event.MouseMove)
            {
                var mouseMoveEventArgs = (MouseMoveEventArgs)baseEventArgs;
                mouseX = (int)mouseMoveEventArgs.MousePosition.x;
                mouseY = (int)mouseMoveEventArgs.MousePosition.y;
                browser.GetBrowserHost().SendMouseMoveEvent(new MouseEvent(mouseX, mouseY, CefEventFlags.None), false);
            }

            base.HandleEvent(eventType, baseEventArgs);
        }
    }
}
