namespace Engine.Renderer.GL.Managers.ImGuiWindows
{
    public interface IImGuiWindow
    {
        bool Render { get; set; }
        string Title { get; }
        string IconGlyph { get; }

        void Draw();
    }
}
