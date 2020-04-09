namespace ECSEngine.Managers.ImGuiWindows
{
    internal interface IImGuiWindow
    {
        bool Render { get; set; }
        string Title { get; }
        string IconGlyph { get; }

        void Draw();
    }
}
