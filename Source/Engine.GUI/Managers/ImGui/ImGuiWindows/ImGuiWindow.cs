using Engine.ECS.Entities;
using Engine.ECS.Observer;
using Engine.Types;
using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;

namespace Engine.GUI.Managers.ImGuiWindows
{
    public abstract class ImGuiWindow : IObserver
    {
        public abstract bool Render { get; set; }
        public abstract string Title { get; }
        public abstract string IconGlyph { get; }
        public virtual ImGuiWindowFlags Flags { get; }
        public IHasParent Parent { get => ImGuiManager.Instance; set { _ = value; } }

        public List<IEntity> Entities => throw new System.NotImplementedException();

        public abstract void Draw();

        public ImGuiWindow()
        {
            Subject.AddObserver(this);
        }

        protected void DrawShadowLabel(string str, Vector2 position)
        {
            var shadowOffset = new Vector2(1, 1);

            ImGui.GetBackgroundDrawList().AddText(
                position + shadowOffset, 0x44000000, str); // Shadow
            ImGui.GetBackgroundDrawList().AddText(
                position, 0xFFFFFFFF, str);
        }

        public virtual void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs) { }
    }
}
