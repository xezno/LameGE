namespace ECSEngine.Managers.ImGuiWindows
{
    internal interface IImGuiWindow
    {
        string Title { get; }
        bool Render { get; set; }
        string IconGlyph { get; }

        void Draw();
    }
}
