using System.Numerics;

namespace Engine.Components
{
    public class BoxColliderComponent : BaseColliderComponent
    {
        public Vector3 Scale { get; set; } = new Vector3(1, 1, 1);
    }
}
