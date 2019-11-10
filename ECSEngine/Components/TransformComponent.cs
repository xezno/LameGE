using ECSEngine.Math;

namespace ECSEngine.Components
{
    public class TransformComponent : Component<TransformComponent>
    {
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public TransformComponent(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
}
