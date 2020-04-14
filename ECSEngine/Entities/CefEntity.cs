using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using ECSEngine.Assets;
using ECSEngine.Components;
using ECSEngine.Events;
using ECSEngine.MathUtils;
using ECSEngine.Render;
using ImGuiNET;
using OpenGL;

namespace ECSEngine.Entities
{
    public sealed class CefEntity : Entity<CefEntity>
    {
        // file://{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Content/UI/index.html
        private string cefFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Content/UI/index.html";
        private Texture2D browserWindowTexture;

        public override string IconGlyph { get; } = FontAwesome5.Wrench;

        private byte[] textureData;
        private Bitmap textureBitmap;
        private bool setupTexture;
        private bool readyToDraw;
        private ChromiumWebBrowser browser;
        DateTime startTime, endTime;
        private bool updateTexture;

        public CefEntity()
        {
            AddComponent(new ShaderComponent(new Shader("Content/UI/Shaders/main.frag", ShaderType.FragmentShader),
                new Shader("Content/UI/Shaders/main.vert", ShaderType.VertexShader)));
            AddComponent(new TransformComponent(new Vector3(0, 0, -2), new Vector3(0, 0, 0), new Vector3(1, 1, 1)));
            AddComponent(new MaterialComponent(new Material("Content/cube.mtl")));
            AddComponent(new MeshComponent("Content/cube.obj"));
            InitBrowser();
        }

        void InitBrowser()
        {
            // setup cef instance
            var browserSettings = new BrowserSettings
            {
                WindowlessFrameRate = 60
            };

            var cefSettings = new CefSettings();
            Cef.Initialize(cefSettings);

            var requestContextSettings = new RequestContextSettings();
            var requestContext = new RequestContext(requestContextSettings);
            browser = new ChromiumWebBrowser(cefFilePath, browserSettings, requestContext);
            browser.Size = new Size((int)RenderSettings.Default.gameResolutionX - 16, (int)RenderSettings.Default.gameResolutionY - 16);

            browser.BrowserInitialized += (sender, args) => { browser.Load(cefFilePath); };

            browser.LoadError += (sender, args) => Debug.Log($"Browser error {args.ErrorCode}");
            
            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                if (args.IsLoading) return;

                browser.LoadingStateChanged -= handler;

                Debug.Log($"CEF has finished loading page {cefFilePath}");
                browser.ScreenshotAsync(true).ContinueWith(SendBitmapToTexture);
            };
            
            browser.LoadingStateChanged += handler;
        }

        private void SetupTextureOnMainThread(byte[] data, Bitmap bitmap)
        {
            Debug.Log("Setting up texture");
            browserWindowTexture = new Texture2D(data, bitmap.Width, bitmap.Height);
            
            Debug.Log("Ready to draw.");

            Debug.Log($"Texture bitmap ptr: {browserWindowTexture.glTexture}");

            GetComponent<MaterialComponent>().materials[0].diffuseTexture = browserWindowTexture;
            setupTexture = false;
            readyToDraw = true;
        }

        private void SendBitmapToTexture(Task<Bitmap> bitmapTask)
        {
            Debug.Log("Got texture");

            bitmapTask.Wait();
            var bitmap = bitmapTask.Result;

            textureData = BitmapToBytes(bitmap);
            textureBitmap = bitmap;
            setupTexture = true;
        }

        private byte[] BitmapToBytes(Bitmap bitmap)
        {
            byte[] data = new byte[bitmap.Width * bitmap.Height * 4];

            int i = 0;

            for (int y = bitmap.Height - 1; y >= 0; --y)
            {
                for (int x = 0; x < bitmap.Width; ++x)
                {
                    var color = bitmap.GetPixel(x, y);
                    data[i] = color.R; // R
                    data[i + 1] = color.G; // G
                    data[i + 2] = color.B; // B
                    data[i + 3] = color.A; // A

                    i += 4;
                }
            }

            return data;
        }

        public override void Render()
        {
            // render cef offscreen & then blit to screen
            // we need to set up texture on the main therad
            // since that wont happen unless we call it here, we need to
            // declare a bool that allows us to detect when we need to
            // setup the texture.

            if (setupTexture)
                SetupTextureOnMainThread(textureData, textureBitmap);

            if (updateTexture)
                UpdateTextureOnMainThread();

            if (!readyToDraw) return;

            // draw to screen
            base.Render();
        }

        public void UpdateBrowserView()
        {
            browser.ScreenshotAsync(true).ContinueWith(UpdateScreenshot);
        }

        private void UpdateScreenshot(Task<Bitmap> bitmapTask)
        {
            startTime = DateTime.Now;
            bitmapTask.Wait();
            var bitmap = bitmapTask.Result;
            textureBitmap = bitmap;
            textureData = BitmapToBytes(bitmap);
            updateTexture = true;
        }

        public void UpdateTextureOnMainThread()
        {
            var textureDataPtr = Marshal.AllocHGlobal(textureData.Length);
            Marshal.Copy(textureData, 0, textureDataPtr, textureData.Length);

            var glTexturePtr = GetComponent<MaterialComponent>().materials[0].diffuseTexture.glTexture;

            Gl.BindTexture(TextureTarget.Texture2d, glTexturePtr);

            Gl.TexSubImage2D(TextureTarget.Texture2d, 0, 0, 0, textureBitmap.Width, textureBitmap.Height, PixelFormat.Rgba, PixelType.UnsignedByte, textureDataPtr);
            Gl.GenerateMipmap(TextureTarget.Texture2d);

            Gl.BindTexture(TextureTarget.Texture2d, 0);

            Marshal.FreeHGlobal(textureDataPtr);
            endTime = DateTime.Now;
            Debug.Log($"Screenshot update took {(endTime - startTime).TotalSeconds}");
            updateTexture = false;
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

        public override void RenderImGui()
        {
            if (ImGui.Button("Update"))
                UpdateBrowserView();
            base.RenderImGui();
        }
    }
}
