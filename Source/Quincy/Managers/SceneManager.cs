using Engine.ECS.Managers;
using Engine.Utils;
using Engine.Utils.MathUtils;
using OpenGL;
using Quincy.Components;
using Quincy.Entities;
using Quincy.Primitives;
using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace Quincy.Managers
{
    public sealed class SceneManager : Manager<SceneManager> // TODO: we probably want to read scene data from a file later.
    {
        private ShaderComponent shader, depthShader;
        private Plane framebufferRenderPlane;
        private ShaderComponent framebufferRenderShader;

        private DateTime lastUpdate;

        private Texture brdfLut;

        private SkyboxEntity skyboxEntity;

        public ImmutableList<CameraEntity> Cameras { get; private set; }

        /// <summary>
        /// The main world camera used to render all entities. Defined as the first object in the Cameras list.
        /// </summary>
        public CameraEntity MainCamera => Cameras[0];

        public ImmutableList<LightEntity> Lights { get; private set; }

        /// <summary>
        /// Construct a world manager containing any entities required.
        /// </summary>
        public SceneManager()
        {
            LoadBaseContent();
            AddBaseEntities();
        }

        private void LoadBaseContent()
        {
            shader = new ShaderComponent("Content/Shaders/PBR/pbr.frag", "Content/Shaders/PBR/pbr.vert");
            depthShader = new ShaderComponent("Content/Shaders/Depth/depth.frag", "Content/Shaders/Depth/depth.vert");

            framebufferRenderShader = new ShaderComponent("Content/Shaders/Framebuffer/framebuffer.frag", "Content/Shaders/Framebuffer/framebuffer.vert");
            framebufferRenderPlane = new Plane();

            brdfLut = new Texture()
            {
                Id = EquirectangularToCubemap.CreateBrdfLut(),
                Path = "brdfLut",
                Type = "texture_lut"
            };
        }

        private void AddBaseEntities()
        {
            // TODO: make this modifiable at run-time or by the game itself.
            Lights = ImmutableList.Create(
                new LightEntity()
                {
                    Name = "Spot Light"
                }
            );

            foreach (var light in Lights)
                AddEntity(light);

            // TODO: make this modifiable at run-time or by the game itself.
            Cameras = ImmutableList.Create(
                new CameraEntity(new Vector3d(0, 2, 0), GameSettings.GameResolutionX, GameSettings.GameResolutionY)
                {
                    Name = "Main Camera"
                }
            );

            foreach (var camera in Cameras)
                AddEntity(camera);

            skyboxEntity = new SkyboxEntity(Constants.hdri);
            AddEntity(skyboxEntity);
        }

        public void Render()
        {
            Update();

            RenderSceneToFramebuffer();
            RenderFramebufferToScreen();
        }

        private void RenderSceneToFramebuffer()
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, MainCamera.GetComponent<CameraComponent>().Framebuffer.Fbo);
            Gl.Viewport(0, 0, GameSettings.GameResolutionX, GameSettings.GameResolutionY);
            Gl.ClearDepth(0.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.ClipControl(ClipControlOrigin.LowerLeft, ClipControlDepth.ZeroToOne);
            Gl.DepthFunc(DepthFunction.Greater);

            MainCamera.Render();
            var skyboxComponent = skyboxEntity.GetComponent<SkyboxComponent>();
            skyboxComponent.DrawSkybox(MainCamera);
            foreach (var entity in Entities)
            {
                if (!entity.HasComponent<ModelComponent>())
                    continue;

                var modelComponent = entity.GetComponent<ModelComponent>();
                modelComponent.Draw(MainCamera, shader, Lights[0], (skyboxComponent.skybox, skyboxComponent.convolutedSkybox, skyboxComponent.prefilteredSkybox), brdfLut);
            }
            // testModel.Draw(camera, shader, light, (skybox, convolutedSkybox, prefilteredSkybox), brdfLut);

            Gl.DepthFunc(DepthFunction.Less);
            Gl.ClipControl(ClipControlOrigin.LowerLeft, ClipControlDepth.NegativeOneToOne);
        }

        private void RenderFramebufferToScreen()
        {
            Gl.Viewport(0, 0, GameSettings.GameResolutionX, GameSettings.GameResolutionY);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Gl.ClearDepth(1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            framebufferRenderShader.SetFloat("exposure", Constants.exposure);
            framebufferRenderPlane.Draw(framebufferRenderShader, MainCamera.GetComponent<CameraComponent>().Framebuffer.ColorTexture);
        }

        public void RenderShadows()
        {
            Gl.Disable(EnableCap.CullFace);

            Gl.Viewport(0, 0, (int)Lights[0].GetComponent<LightComponent>().ShadowMap.Resolution.x, (int)Lights[0].GetComponent<LightComponent>().ShadowMap.Resolution.y);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Lights[0].GetComponent<LightComponent>().ShadowMap.DepthMapFbo);
            Gl.Clear(ClearBufferMask.DepthBufferBit);

            Lights[0].Render();
            foreach (var entity in Entities)
            {
                if (!entity.HasComponent<ModelComponent>())
                    continue;

                var modelComponent = entity.GetComponent<ModelComponent>();
                modelComponent.DrawShadows(Lights[0].GetComponent<LightComponent>(), depthShader);
            }
            // testModel.DrawShadows(light.GetComponent<LightComponent>(), depthShader);

            Gl.Enable(EnableCap.CullFace);

            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Update()
        {
            float deltaTime = (float)(DateTime.Now - lastUpdate).TotalSeconds;
            foreach (var entity in Entities)
            {
                if (!entity.HasComponent<ModelComponent>())
                    continue;

                var modelComponent = entity.GetComponent<ModelComponent>();
                modelComponent.Update(deltaTime);
            }
            // testModel.Update(deltaTime);
            MainCamera.Update(deltaTime);

            lastUpdate = DateTime.Now;
        }
    }
}