using ECSEngine.Managers.ImGuiWindows;
using System.Collections.Generic;

namespace ECSEngine.Managers
{
    public class ImGuiMenu
    {
        public string Title { get; }
        public string IconGlyph { get; }

        public List<IImGuiWindow> windows;

        public ImGuiMenu(string iconGlyph, string title, List<IImGuiWindow> windows)
        {
            Title = title;
            IconGlyph = iconGlyph;
            this.windows = windows;
        }
    }
}
