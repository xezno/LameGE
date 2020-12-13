using System;

namespace Engine.GUI.Managers.ImGuiWindows
{
    internal class ImGuiMenuPathAttribute : Attribute
    {
        public ImGuiMenus.Menu Path { get; }
        public ImGuiMenuPathAttribute(ImGuiMenus.Menu path)
        {
            Path = path;
        }
    }
}