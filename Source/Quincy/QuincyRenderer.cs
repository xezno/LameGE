using OpenGL;

namespace Quincy
{
    public class QuincyRenderer
    {
        private Scene scene;

        public QuincyRenderer() { }

        public void ContextCreated()
        {
            Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.Blend);
            Gl.Enable(EnableCap.CullFace);
            Gl.Enable(EnableCap.FramebufferSrgb);
            Gl.Enable((EnableCap)Gl.TEXTURE_CUBE_MAP_SEAMLESS);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            scene = new Scene();
        }

        public void RenderImGui() { }

        public void RenderToScreen()
        {
            scene.Render();
        }

        public void RenderToShadowMap()
        {
            scene.RenderShadows();
        }
    }
}
