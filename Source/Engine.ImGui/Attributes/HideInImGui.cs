using System;

namespace Engine.Gui.Attributes
{
    /// <summary>
    /// Prevents an item from being shown in ImGui windows.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class HideInImGui : Attribute { }
}
