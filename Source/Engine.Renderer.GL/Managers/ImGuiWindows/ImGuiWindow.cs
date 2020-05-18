using Engine.Events;
using ImGuiNET;
using System.Numerics;

namespace Engine.Renderer.GL.Managers.ImGuiWindows
{
    public abstract class ImGuiWindow
    {
        public abstract bool Render { get; set; }
        public abstract string Title { get; }
        public abstract string IconGlyph { get; }
        public abstract void Draw();

        protected void DrawShadowLabel(string str, Vector2 position)
        {
            var shadowOffset = new Vector2(1, 1);

            ImGui.GetBackgroundDrawList().AddText(
                position + shadowOffset, 0x44000000, str); // Shadow
            ImGui.GetBackgroundDrawList().AddText(
                position, 0xFFFFFFFF, str);
        }
        
        public virtual void HandleEvent(Event eventType, IEventArgs baseEventArgs) { }
    }
}
