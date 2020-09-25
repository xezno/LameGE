using Engine.Utils.MathUtils;
using System;
using System.Numerics;

namespace Engine.Gui.Managers.ImGuiWindows.Editor.NodeEditor
{
    public class Node
    {
        public Node(int id, string name, Vector2 position, Vector2 size, float value, Vector4 color, int inputCount, int outputCount)
        {
            Id = id;
            Name = name;
            Position = position;
            Size = size;
            Value = value;
            Color = color;
            InputCount = inputCount;
            OutputCount = outputCount;
        }

        public Vector2 GetInputSlotPos(int slotNumber)
        {
            return new Vector2(Position.X, Position.Y + Size.Y * ((float)slotNumber + 1) / ((float)InputCount + 1));
        }

        public Vector2 GetOutputSlotPos(int slotNumber)
        {
            return new Vector2(Position.X + Size.X, Position.Y + Size.Y * ((float)slotNumber + 1) / ((float)OutputCount + 1));
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public float Value { get; set; }
        public Vector4 Color { get; set; }
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
    }
}
