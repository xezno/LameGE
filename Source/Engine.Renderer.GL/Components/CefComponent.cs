using CefSharp;
using CefSharp.OffScreen;
using CefSharp.Structs;
using Engine.ECS.Components;
using Engine.ECS.Observer;
using Engine.Renderer.GL.Entities.Cef;
using Engine.Renderer.GL.Render;
using Engine.Utils;
using Engine.Utils.DebugUtils;
using OpenGL;
using System;

namespace Engine.Renderer.GL.Components
{
    public class CefComponent : Component<CefComponent>
    {
        private string cefFilePath = $"";

        public string CefFilePath
        {
            get => cefFilePath;
            set
            {
                cefFilePath = value;
                browser.Load(cefFilePath);
            }
        }

        public bool ReadyToDraw { get; private set; }
        private ChromiumWebBrowser browser;

        private int mouseX, mouseY;
        private CefRenderHandler renderHandler;

        private Texture2D modifiedTexture;

        // TODO: Prevent tonemapping from occurring while rendering

        public CefComponent(string cefFilePath)
        {
            this.cefFilePath = cefFilePath;
        }
        
        void InitBrowser()
        {
            // setup cef instance
            var browserSettings = new BrowserSettings
            {
                WindowlessFrameRate = 30, // 30 works best here as it prevents flickering, but this might be system dependant - TODO: needs experimentation
            };

            var cefSettings = new CefSettings();
            cefSettings.SetOffScreenRenderingBestPerformanceArgs();
            Cef.Initialize(cefSettings);

            var requestContextSettings = new RequestContextSettings();
            var requestContext = new RequestContext(requestContextSettings);
            browser = new ChromiumWebBrowser(cefFilePath, browserSettings, requestContext);
            browser.Size = new System.Drawing.Size(GameSettings.GameResolutionX, GameSettings.GameResolutionY);
            browser.RenderHandler = new CefRenderHandler(browser);
            browser.BrowserInitialized += (sender, args) => { browser.Load(cefFilePath); };
            browser.LoadError += (sender, args) => Logging.Log($"Browser error {args.ErrorCode}");

            renderHandler = (CefRenderHandler)browser.RenderHandler;

            byte[] emptyData = new byte[browser.Size.Width * browser.Size.Height * 4];
            GetComponent<MaterialComponent>().materials[0].diffuseTexture =
                new Texture2D(emptyData, browser.Size.Width, browser.Size.Height);

            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                if (args.IsLoading) return;

                browser.LoadingStateChanged -= handler;

                Logging.Log($"CEF has finished loading page {cefFilePath}");
                ReadyToDraw = true;
            };

            browser.LoadingStateChanged += handler;

            modifiedTexture = new Texture2D(IntPtr.Zero, GameSettings.GameResolutionX, GameSettings.GameResolutionY, 32);
        }

        public void SetTextureData()
        {
            if (!renderHandler.NeedsPaint)
                return;

            Paint(renderHandler.Type, renderHandler.DirtyRect, renderHandler.Buffer, renderHandler.Width, renderHandler.Height);
            renderHandler.NeedsPaint = false;
            
            GetComponent<MaterialComponent>().materials[0].diffuseTexture = modifiedTexture;
        }

        private void Paint(PaintElementType type, Rect dirtyRect, IntPtr buffer, int width, int height)
        {
            Gl.BindTexture(TextureTarget.Texture2d, modifiedTexture.glTexture);

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

        public override void OnNotify(NotifyType eventType, INotifyArgs notifyArgs)
        {
            switch (eventType)
            {
                case NotifyType.GameEnd:
                    browser.Dispose();
                    break;
                // TODO: please no
                case NotifyType.GameStart:
                    InitBrowser();
                    break;
                case NotifyType.MouseButtonDown:
                    {
                        var mouseEventArgs = (MouseButtonNotifyArgs)notifyArgs;
                        var mouseButtonType = mouseEventArgs.MouseButton == 0 ? MouseButtonType.Left : MouseButtonType.Right;
                        var eventFlags = mouseButtonType == MouseButtonType.Left ? CefEventFlags.LeftMouseButton : CefEventFlags.RightMouseButton;
                        browser.GetBrowserHost().SendMouseClickEvent(new MouseEvent(mouseX, mouseY, eventFlags), mouseButtonType, false, 0);
                        break;
                    }

                case NotifyType.MouseButtonUp:
                    {
                        var mouseEventArgs = (MouseButtonNotifyArgs)notifyArgs;
                        var mouseButtonType = mouseEventArgs.MouseButton == 0 ? MouseButtonType.Left : MouseButtonType.Right;
                        var eventFlags = mouseButtonType == MouseButtonType.Left ? CefEventFlags.LeftMouseButton : CefEventFlags.RightMouseButton;
                        browser.GetBrowserHost().SendMouseClickEvent(new MouseEvent(mouseX, mouseY, eventFlags), mouseButtonType, true, 0);
                        break;
                    }

                case NotifyType.MouseMove:
                    {
                        var mouseMoveEventArgs = (MouseMoveNotifyArgs)notifyArgs;
                        mouseX = (int)mouseMoveEventArgs.MousePosition.x;
                        mouseY = (int)mouseMoveEventArgs.MousePosition.y;
                        browser.GetBrowserHost().SendMouseMoveEvent(new MouseEvent(mouseX, mouseY, CefEventFlags.None), false);
                        break;
                    }
            }

            base.OnNotify(eventType, notifyArgs);
        }
    }
}
