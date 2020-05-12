using CefSharp;
using CefSharp.OffScreen;
using CefSharp.Structs;
using System;

namespace Engine.Entities.CEF
{
    class RenderHandler : DefaultRenderHandler, IDisposable
    {
        public bool NeedsPaint { get; set; }
        public PaintElementType Type { get; set; }
        public Rect DirtyRect { get; set; }
        public IntPtr Buffer { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public RenderHandler(ChromiumWebBrowser browser) : base(browser) { }

        public override void OnAcceleratedPaint(PaintElementType type, Rect dirtyRect, IntPtr sharedHandle)
        {
            base.OnAcceleratedPaint(type, dirtyRect, sharedHandle);
            throw new NotImplementedException();
        }

        public override void OnPaint(PaintElementType type, Rect dirtyRect, IntPtr buffer, int width, int height)
        {
            Type = type;
            DirtyRect = dirtyRect;
            Buffer = buffer;
            Width = width;
            Height = height;
            NeedsPaint = true;
            base.OnPaint(type, dirtyRect, buffer, width, height);
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}
