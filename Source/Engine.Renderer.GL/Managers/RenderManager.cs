using Engine.Components;
using Engine.ECS.Entities;
using Engine.ECS.Managers;
using Engine.Renderer.GL.Components;
using Engine.Renderer.GL.Entities;
using Engine.Renderer.GL.Render;
using Engine.Utils;
using Engine.Utils.MathUtils;
using OpenGL;
using System;
using System.Threading;

namespace Engine.Renderer.GL.Managers
{
    public class RenderManager : Manager<RenderManager>
    {
        private DateTime lastRender;
        private int currentFrametimeIndex;
        private int currentFramerateIndex;

        private const int FramesToCount = 480;
        private readonly Renderer renderer;

        private ShaderComponent shadowShaders;
        private float framerateLimitAsMs = 1000f / GameSettings.FramerateLimit;

        public float LastFrameTime { get; private set; }
        public int CalculatedFramerate => (int)(1000f / Math.Max(LastFrameTime, 0.001f));
        public float[] FrametimeHistory { get; } = new float[FramesToCount];
        public float[] FramerateHistory { get; } = new float[FramesToCount];

        public bool RenderShadowMap { get; set; }
        public bool Paused { get; set; }

        public RenderManager()
        {
            renderer = new Renderer();
            renderer.Init();
            shadowShaders = new ShaderComponent(new Shader("Content/Shaders/Depth/depth.frag", Shader.Type.FragmentShader),
                new Shader("Content/Shaders/Depth/depth.vert", Shader.Type.VertexShader));
        }

        private void RenderScene(Matrix4x4f projMatrix, Matrix4x4f viewMatrix, Vector3d cameraPosition)
        {
            foreach (var entity in SceneManager.Instance.Entities)
            {
                if (entity.Enabled)
                {
                    if (entity.HasComponent<MeshComponent>())
                    {
                        RenderMesh(entity, projMatrix, viewMatrix, cameraPosition);
                    }
                }
            }
        }

        private void RenderLights(LightComponent lightComponent)
        {
            foreach (var entity in SceneManager.Instance.Entities)
            {
                if (entity.Enabled)
                {
                    if (entity.HasComponent<MeshComponent>())
                    {
                        RenderMeshShadow(entity, lightComponent.lightMatrix);
                    }
                }
            }
        }

        public void RenderCefComponent(CefComponent cefComponent)
        {
            // render cef offscreen & then blit to screen
            // we need to set up texture on the main therad
            // since that wont happen unless we call it here, we need to
            // declare a bool that allows us to detect when we need to
            // setup the texture.

            if (!cefComponent.ReadyToDraw) return;

            Gl.Disable(EnableCap.DepthTest);

            // draw to screen
            cefComponent.Render();

            cefComponent.SetTextureData();

            Gl.Enable(EnableCap.DepthTest);
        }

        private void RenderCef()
        {
            foreach (var entity in SceneManager.Instance.Entities)
            {
                if (entity.Enabled && entity.HasComponent<CefComponent>())
                {
                    RenderCefComponent(entity.GetComponent<CefComponent>());
                }
            }
        }

        private void BindMatrices(ShaderComponent shaderComponent, Matrix4x4f projMatrix, Matrix4x4f viewMatrix)
        {
            shaderComponent.SetVariable("projMatrix", projMatrix);
            shaderComponent.SetVariable("viewMatrix", viewMatrix);
        }

        private void RenderMeshShadow(IEntity entity, Matrix4x4f lightMatrix)
        {
            var transformComponent = entity.GetComponent<TransformComponent>();
            var meshComponent = entity.GetComponent<MeshComponent>();

            shadowShaders.UseShader();

            Gl.BindVertexArray(meshComponent.RenderMesh.vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, meshComponent.RenderMesh.vbo);

            shadowShaders.SetVariable("lightMatrix", lightMatrix);
            shadowShaders.SetVariable("modelMatrix", transformComponent.Matrix);

            Gl.DrawArrays(PrimitiveType.Triangles, 0, meshComponent.RenderMesh.ElementCount * sizeof(float));
            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void RenderMesh(IEntity entity, Matrix4x4f projMatrix, Matrix4x4f viewMatrix, Vector3d cameraPosition)
        {
            var shaderComponent = entity.GetComponent<ShaderComponent>();
            var transformComponent = entity.GetComponent<TransformComponent>();
            var meshComponent = entity.GetComponent<MeshComponent>();

            shaderComponent.UseShader();

            Gl.BindVertexArray(meshComponent.RenderMesh.vao);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, meshComponent.RenderMesh.vbo);

            BindMatrices(shaderComponent, projMatrix, viewMatrix);

            shaderComponent.SetVariable("cameraPos", cameraPosition.ToVector3());
            shaderComponent.SetVariable("modelMatrix", transformComponent.Matrix);
            shaderComponent.SetVariable("fogNear", 0.02f);
            shaderComponent.SetVariable("skyColor", new Vector3f(100 / 255f, 149 / 255f, 237 / 255f)); // Cornflower blue

            entity.GetComponent<MaterialComponent>().BindAll(shaderComponent);

            SceneManager.Instance.Lights[0].GetComponent<LightComponent>().Bind(shaderComponent);

            Gl.DrawArrays(PrimitiveType.Triangles, 0, meshComponent.RenderMesh.ElementCount * sizeof(float));

            Gl.BindVertexArray(0);
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        /// <summary>
        /// Render all the entities within the render manager.
        /// </summary>
        public override void Run()
        {
            Gl.ClipControl(ClipControlOrigin.LowerLeft, ClipControlDepth.ZeroToOne);
            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Greater);
            RenderLighting();
            RenderCameras();
            Gl.ClipControl(ClipControlOrigin.LowerLeft, ClipControlDepth.NegativeOneToOne);
            Gl.DepthFunc(DepthFunction.Less);
            Gl.Disable(EnableCap.DepthTest);
            
            SceneManager.Instance.MainCamera.GetComponent<CameraComponent>().Framebuffer.Render();
            
            // DEBUG: Render shadow map to display
            if (RenderShadowMap)
                SceneManager.Instance.Lights[0].GetComponent<LightComponent>().shadowMap.Render();

            renderer.FinishRender();
            
            RenderCef();
            CollectPerformanceData();
        }

        public void RenderLighting()
        {
            // Render lighting
            foreach (var lightEntity in SceneManager.Instance.Lights)
            {
                RenderAsLight(lightEntity);
            }

        }

        public void RenderCameras()
        {
            // Render scene
            foreach (var cameraEntity in SceneManager.Instance.Cameras)
            {
                RenderAsCamera(cameraEntity);
            }
        }

        public void RenderAsLight(LightEntity lightEntity)
        {
            // Render scene from light (shadow map)        
            var lightComponent = lightEntity.GetComponent<LightComponent>();

            lightComponent.shadowMap.Bind();
            RenderLights(lightComponent);
            lightComponent.shadowMap.Unbind();
        }

        public void RenderAsCamera(CameraEntity cameraEntity)
        {
            var cameraComponent = cameraEntity.GetComponent<CameraComponent>();

            renderer.PrepareRender();
            cameraComponent.Framebuffer.Bind();
            renderer.PrepareFramebufferRender();

            RenderScene(cameraComponent.ProjMatrix, cameraComponent.ViewMatrix, cameraEntity.GetComponent<TransformComponent>().Position);

            cameraComponent.Framebuffer.Unbind();
        }

        public void CollectPerformanceData()
        {
            LastFrameTime = (DateTime.Now - lastRender).Milliseconds;

            if (!Paused)
            {
                FrametimeHistory[currentFrametimeIndex++] = LastFrameTime;

                if (currentFrametimeIndex == FrametimeHistory.Length)
                {
                    currentFrametimeIndex--;
                    for (var i = 0; i < FrametimeHistory.Length; ++i)
                        FrametimeHistory[i] = FrametimeHistory[(i + 1) % FrametimeHistory.Length];
                }

                FramerateHistory[currentFramerateIndex++] = CalculatedFramerate;
                if (currentFramerateIndex == FramerateHistory.Length)
                {
                    currentFramerateIndex--;
                    for (var i = 0; i < FramerateHistory.Length; ++i)
                        FramerateHistory[i] = FramerateHistory[(i + 1) % FramerateHistory.Length];
                }
            }

            lastRender = DateTime.Now;

            // Slow down rendering if it's going past the framerate limit
            if (LastFrameTime < framerateLimitAsMs && GameSettings.FramerateLimit > 0)
            {
                var nextFrameDelay = (int)Math.Ceiling(framerateLimitAsMs - LastFrameTime);
                Thread.Sleep(nextFrameDelay);
            }
        }
    }
}
