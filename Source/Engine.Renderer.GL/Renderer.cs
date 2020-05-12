using Engine.Utils.DebugUtils;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Engine.Renderer.GL
{
    public class Renderer
    {
        private readonly Gl.DebugProc debugCallback; // Stored to prevent GC from collecting debug callback before it can be called

        public Renderer()
        {

            debugCallback = DebugCallback;
        }

        public void Init()
        {
            Logging.Log($"OpenGL {Gl.GetString(StringName.Version)}");
            CheckHardwareCompatibility();
            Gl.ReadBuffer(ReadBufferMode.Back);
            Gl.Enable(EnableCap.Blend);
            Gl.Enable(EnableCap.CullFace);
            Gl.Enable(EnableCap.DepthTest);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Gl.DebugMessageCallback(debugCallback, IntPtr.Zero);
        }

        public void PrepareRender()
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Clear non-fb
        }

        public void PrepareFramebufferRender()
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Clear fb
        }

        public void FinishRender()
        {
            Gl.Finish();
        }

        public void SetViewportSize(int width, int height)
        {
            Gl.Viewport(0, 0, width, height);
        }

        private void DebugCallback(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            if (severity >= DebugSeverity.DebugSeverityMedium)
                Logging.Log($"OpenGL Error {id}: {Marshal.PtrToStringAnsi(message, length)}", Logging.Severity.Fatal);
        }

        private void CheckHardwareCompatibility()
        {
            var requiredExtensions = new[] { "GL_ARB_spirv_extensions" };
            var existingExtensions = new List<String>();

            var extensionCount = 0;
            Gl.GetInteger(GetPName.NumExtensions, out extensionCount);

            for (int i = 0; i < extensionCount; ++i)
            {
                existingExtensions.Add(Gl.GetString(StringName.Extensions, (uint)i));
            }

            foreach (var extension in requiredExtensions)
            {
                if (!existingExtensions.Contains(extension))
                {
                    Logging.Log($"GPU does not support extension {extension}", Logging.Severity.Fatal);
                }
            }
        }
    }
}
