using Engine.Common.DebugUtils;

namespace Engine.Common.Base
{
    public interface IRenderer
    {
        void ContextCreated();
        void RenderImGui();
        void RenderToScreen();
        void RenderToShadowMap();
    }

    public class NullRenderer : IRenderer
    {
        public void ContextCreated()
        {
            Logging.Log("The null renderer is in use; therefore, no graphics will be displayed.", Logging.Severity.Medium);
        }

        public void RenderImGui()
        {
            
        }

        public void RenderToScreen()
        {
            
        }

        public void RenderToShadowMap()
        {
            
        }
    }
}
