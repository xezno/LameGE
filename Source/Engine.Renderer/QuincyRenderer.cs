using Engine.Renderer.Managers;
using Engine.Utils.Base;
using OpenGL;

namespace Engine.Renderer
{
    public class QuincyRenderer : IRenderer
    {
        public QuincyRenderer() { }

        public void ContextCreated()
        {
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.Blend);
            Gl.Enable(EnableCap.CullFace);
            Gl.Enable(EnableCap.FramebufferSrgb);
            Gl.Enable((EnableCap)Gl.TEXTURE_CUBE_MAP_SEAMLESS);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public void RenderImGui() { }

        public void RenderToScreen()
        {
            SceneManager.Instance.Render();
        }

        public void RenderToShadowMap()
        {
            SceneManager.Instance.RenderShadows();
        }
    }
}
