using CefSharp;
using CefSharp.OffScreen;
using CefSharp.Structs;
using ECSEngine.Assets;
using ECSEngine.Components;
using ECSEngine.Entities.CEF;
using ECSEngine.Events;
using ECSEngine.MathUtils;
using ECSEngine.Render;
using OpenGL;
using System;
using System.IO;
using System.Reflection;
using Size = System.Drawing.Size;

namespace ECSEngine.Entities
{
    // TODO: Move to component, render to a texture and make a HudEntity instead
    public sealed class CefEntity : Entity<CefEntity>
    {
        private string cefFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Content/UI/index.html";

        public override string IconGlyph { get; } = FontAwesome5.Wrench;
        private bool readyToDraw;
        private ChromiumWebBrowser browser;

        public CefEntity()
        {
            AddComponent(new ShaderComponent(new Shader("Content/UI/Shaders/main.frag", ShaderType.FragmentShader),
                new Shader("Content/UI/Shaders/main.vert", ShaderType.VertexShader)));
            AddComponent(new TransformComponent(new Vector3(0, 0, -2), new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
            AddComponent(new MaterialComponent(new Material("Content/Materials/cube.mtl")));
            AddComponent(new MeshComponent("Content/Models/cube.obj"));
            InitBrowser();
        }

        void InitBrowser()
        {
            // setup cef instance
            var browserSettings = new BrowserSettings
            {
                WindowlessFrameRate = 120
            };

            var cefSettings = new CefSettings();
            Cef.Initialize(cefSettings);

            var requestContextSettings = new RequestContextSettings();
            var requestContext = new RequestContext(requestContextSettings);
            browser = new ChromiumWebBrowser(cefFilePath, browserSettings, requestContext);
            browser.Size = new Size((int)GameSettings.Default.gameResolutionX - 16, (int)GameSettings.Default.gameResolutionY - 16);
            browser.RenderHandler = new CEF.RenderHandler(browser);

            browser.BrowserInitialized += (sender, args) => { browser.Load(cefFilePath); };
            browser.LoadError += (sender, args) => Debug.Logging.Log($"Browser error {args.ErrorCode}");

            byte[] emptyData = new byte[browser.Size.Width * browser.Size.Height * 4];
            GetComponent<MaterialComponent>().materials[0].diffuseTexture =
                new Texture2D(emptyData, browser.Size.Width, browser.Size.Height);

            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                if (args.IsLoading) return;

                browser.LoadingStateChanged -= handler;

                Debug.Logging.Log($"CEF has finished loading page {cefFilePath}");
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

            // if (!readyToDraw) return;

            SetTextureData();
            // draw to screen
            base.Render();
        }

        private void SetTextureData()
        {
            var renderHandler = ((RenderHandler)browser.RenderHandler);
            if (!renderHandler.NeedsPaint)
                return;

            Paint(renderHandler.Type, renderHandler.DirtyRect, renderHandler.Buffer, renderHandler.Width, renderHandler.Height);
            renderHandler.NeedsPaint = false;
        }

        private void Paint(PaintElementType type, Rect dirtyRect, IntPtr buffer, int width, int height)
        {
            Gl.BindTexture(TextureTarget.Texture2d, GetComponent<MaterialComponent>().materials[0].diffuseTexture.glTexture);

            Gl.TexSubImage2D(TextureTarget.Texture2d, 0, dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height, PixelFormat.Bgra, PixelType.UnsignedByte, buffer);
            Gl.GenerateMipmap(TextureTarget.Texture2d);

            Gl.BindTexture(TextureTarget.Texture2d, 0);
        }

        public override void HandleEvent(Event eventType, IEventArgs baseEventArgs)
        {
            if (eventType == Event.GameEnd)
            {
                //browser.Dispose();
                //requestContext.Dispose();
            }

            base.HandleEvent(eventType, baseEventArgs);
        }
    }
}
