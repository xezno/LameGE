using Engine.Utils.DebugUtils;

namespace Engine.Utils.Base
{
    /// <summary>
    /// An interface specifying the minimum contract required for a renderer to attach to the engine.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Called when the window's renderer context has been created.
        /// </summary>
        void ContextCreated();

        /// <summary>
        /// Called when ImGui is to be rendered.
        /// </summary>
        void RenderImGui();

        /// <summary>
        /// Called when objects are to be rendered to the screen.
        /// </summary>
        void RenderToScreen();

        /// <summary>
        /// Called when objects are to be rendered to the shadow map.
        /// </summary>
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
