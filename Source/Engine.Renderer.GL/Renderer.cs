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

        /// <summary>
        /// Setup the renderer.
        /// </summary>
        public void Init()
        {
            Logging.Log($"OpenGL {Gl.GetString(StringName.Version)}");
            CheckHardwareCompatibility();
            Gl.ReadBuffer(ReadBufferMode.Back);
            Gl.Enable(EnableCap.Blend);
            Gl.Enable(EnableCap.CullFace);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Gl.DebugMessageCallback(debugCallback, IntPtr.Zero);
        }

        /// <summary>
        /// Prepare to render the scene (non-framebuffer).
        /// </summary>
        public void PrepareRender()
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Clear non-fb
        }

        /// <summary>
        /// Prepare to render a Framebuffer.
        /// </summary>
        public void PrepareFramebufferRender()
        {
            Gl.ClearDepth(0.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Clear fb
        }

        /// <summary>
        /// Finish a render.
        /// </summary>
        public void FinishRender()
        {
            Gl.Finish();
        }

        // TODO: Move to CameraComponent or Framebuffer?
        public void SetViewportSize(int width, int height)
        {
            Gl.Viewport(0, 0, width, height);
        }

        /// <summary>
        /// A callback method for the logging of any errors that OpenGL encounters.
        /// </summary>
        /// <param name="source">The source of the callback.</param>
        /// <param name="type">The type of the log.</param>
        /// <param name="id">An identifier for the error.</param>
        /// <param name="severity">The severity of the log (from "Notification" to "High").</param>
        /// <param name="length">The length of the log string.</param>
        /// <param name="message">A pointer to the log string.</param>
        /// <param name="userParam">A user-supplied pointer to be passed on each invocation of the callback.</param>
        private void DebugCallback(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            var logSeverity = Logging.Severity.Low;
            switch (severity)
            {
                case DebugSeverity.DebugSeverityNotification:
                    return; // Ignore
                case DebugSeverity.DebugSeverityLow:
                    logSeverity = Logging.Severity.Low;
                    break;
                case DebugSeverity.DebugSeverityMedium:
                    logSeverity = Logging.Severity.Medium;
                    break;
                case DebugSeverity.DebugSeverityHigh:
                    logSeverity = Logging.Severity.High;
                    break;
                case DebugSeverity.DontCare:
                    logSeverity = Logging.Severity.Low;
                    break;
            }
            Logging.Log($"OpenGL Error {id}: {Marshal.PtrToStringAnsi(message, length)}", logSeverity);
        }

        /// <summary>
        /// Check for any necessary OpenGL extensions.
        /// </summary>
        private void CheckHardwareCompatibility()
        {
            var requiredExtensions = new[] { 
                "GL_ARB_spirv_extensions" /* SPIR-V compilation */,
                "GL_ARB_clip_control" /* Reversed Z-buffer */ 
            };
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
