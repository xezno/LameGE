using ECSEngine.Math;

namespace ECSEngine.Components
{
    public class TransformComponent : Component<TransformComponent>
    {
        public Vector3 position;
        public Quaternion rotation;
        public TransformComponent(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
}
