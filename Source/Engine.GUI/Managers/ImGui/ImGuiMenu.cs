using Engine.Assets;
using Engine.GUI.Managers.ImGuiWindows;
using System.Collections.Generic;

namespace Engine.GUI.Managers
{
    public class ImGuiMenu
    {
        public ImGuiMenus.Menu Menu { get; }
        public string IconGlyph { get; }

        public List<ImGuiWindow> Windows { get; }

        public ImGuiMenu(string iconGlyph, ImGuiMenus.Menu menu, List<ImGuiWindow> windows)
        {
            this.IconGlyph = iconGlyph;
            this.Menu = menu;
            this.Windows = windows;
        }
    }

    public class ImGuiMenus
    {
        // not the most elegant solution, but it works

        public enum Menu
        {
            File,
            Engine,
            Experimental,
            Scene
        }

        public static Dictionary<Menu, string> Icons => new Dictionary<Menu, string>()
        {
            { Menu.File, FontAwesome5.File },
            { Menu.Engine, FontAwesome5.Wrench },
            { Menu.Experimental, FontAwesome5.Magic },
            { Menu.Scene, FontAwesome5.Cube },
        };
    }
}
