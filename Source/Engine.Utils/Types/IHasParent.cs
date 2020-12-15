namespace Engine.Types
{
    /// <summary>
    /// Any class that is able to have a parent.
    /// </summary>
    public interface IHasParent
    {
        /// <summary>
        /// The class's parent.
        /// </summary>
        IHasParent Parent { get; set; }

        /// <summary>
        /// Display the available components & their properties
        /// within ImGUI.
        /// </summary>
        void RenderImGui();
    }
}
