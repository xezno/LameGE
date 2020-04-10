namespace ECSEngine.Managers.ImGuiWindows
{
    public interface IImGuiWindow
    {
        bool Render { get; set; }
        string Title { get; }
        string IconGlyph { get; }

        void Draw();
    }
}
