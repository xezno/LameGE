﻿using Engine.ECS.Managers;
using Engine.Renderer.Components;
using Engine.Renderer.Entities;
using Engine.Renderer.Primitives;
using Engine.Utils;
using Engine.Utils.MathUtils;
using OpenGL;
using System;
using System.Collections.Immutable;
using System.Numerics;

namespace Engine.Renderer.Managers
{
    // TODO: we probably want to read scene data from a file later.
    public sealed class SceneManager : Manager<SceneManager>
    {
        private ShaderComponent depthShader;
        private Primitives.Plane framebufferRenderPlane;
        private ShaderComponent framebufferRenderShader;

        private DateTime lastUpdate;

        private Texture brdfLut;
        private Texture holoTexture;

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
            var fs = ServiceLocator.FileSystem;
            depthShader = new ShaderComponent(fs.GetAsset("/Shaders/Depth/depth.frag"), fs.GetAsset("/Shaders/Depth/depth.vert"));

            framebufferRenderShader = new ShaderComponent(fs.GetAsset("/Shaders/Framebuffer/framebuffer.frag"), fs.GetAsset("/Shaders/Framebuffer/framebuffer.vert"));
            framebufferRenderPlane = new Primitives.Plane();

            brdfLut = new Texture()
            {
                Id = EquirectangularToCubemap.CreateBrdfLut(),
                Path = "brdfLut",
                Type = "texture_lut"
            };

            holoTexture = Texture.LoadFromAsset(fs.GetAsset("/Textures/holoMap.png"), "texture_diffuse");
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
                new CameraEntity(new Vector3(0, 2, 0))
                {
                    Name = "Main Camera"
                }
            );

            foreach (var camera in Cameras)
                AddEntity(camera);

            skyboxEntity = new SkyboxEntity(RenderManager.Instance.hdri);
            AddEntity(skyboxEntity);
        }

        public void Render()
        {
            Update();

            RenderSceneToFramebuffer();
        }

        private void RenderSceneToFramebuffer()
        {
            var cameraComponent = MainCamera.GetComponent<CameraComponent>();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, cameraComponent.Framebuffer.Fbo);
            Gl.Viewport(0, 0, (int)cameraComponent.Resolution.X, (int)cameraComponent.Resolution.Y);
            Gl.ClearDepth(0.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Gl.ClipControl(ClipControlOrigin.LowerLeft, ClipControlDepth.ZeroToOne);
            Gl.DepthFunc(DepthFunction.Greater);

            MainCamera.Render();
            var skyboxComponent = skyboxEntity.GetComponent<SkyboxComponent>();
            skyboxComponent.DrawSkybox(MainCamera);

            var templateDrawInfo = new Mesh.DrawInfo()
            {
                Camera = MainCamera,
                Light = Lights[0],
                PbrCubemaps = (skyboxComponent.skybox, skyboxComponent.convolutedSkybox, skyboxComponent.prefilteredSkybox),
                BrdfLut = brdfLut,
                HoloTexture = holoTexture,
                ProjMatrix = MainCamera.GetComponent<CameraComponent>().ProjMatrix,
                ViewMatrix = MainCamera.GetComponent<CameraComponent>().ViewMatrix
            };

            foreach (var entity in Entities)
            {
                if (!entity.HasComponent<ModelComponent>())
                    continue;

                var modelComponent = entity.GetComponent<ModelComponent>();
                var newDrawInfo = new Mesh.DrawInfo(templateDrawInfo);
                newDrawInfo.Shader = entity.GetComponent<ShaderComponent>();
                newDrawInfo.ModelMatrix = entity.GetComponent<TransformComponent>().Matrix;

                modelComponent.Draw(newDrawInfo);
            }

            Gl.DepthFunc(DepthFunction.Less);
            Gl.ClipControl(ClipControlOrigin.LowerLeft, ClipControlDepth.NegativeOneToOne);
        }

        public void RenderFramebuffer()
        {
            var cameraComponent = MainCamera.GetComponent<CameraComponent>();
            Gl.Viewport(0, 0, GameSettings.GameResolutionX, GameSettings.GameResolutionY);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Gl.ClearDepth(1.0f);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            framebufferRenderShader.SetFloat("exposure", RenderManager.Instance.exposure);
            framebufferRenderShader.SetInt("tonemapOperator", (int)RenderManager.Instance.tonemapOperator);
            framebufferRenderPlane.Draw(framebufferRenderShader, cameraComponent.Framebuffer.ColorTexture);
        }

        public void RenderShadows()
        {
            Gl.Disable(EnableCap.CullFace);

            Gl.Viewport(0, 0, (int)Lights[0].GetComponent<LightComponent>().ShadowMap.Resolution.X, (int)Lights[0].GetComponent<LightComponent>().ShadowMap.Resolution.Y);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, Lights[0].GetComponent<LightComponent>().ShadowMap.DepthMapFbo);
            Gl.Clear(ClearBufferMask.DepthBufferBit);

            foreach (var entity in Entities)
            {
                if (!entity.HasComponent<ModelComponent>())
                    continue;

                var modelComponent = entity.GetComponent<ModelComponent>();
                modelComponent.DrawShadows(Lights[0].GetComponent<LightComponent>(), depthShader);
            }

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
            Lights[0].Update(deltaTime);
            MainCamera.Update(deltaTime);

            lastUpdate = DateTime.Now;
        }
    }
}