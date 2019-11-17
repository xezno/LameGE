using ECSEngine.Math;

namespace ECSEngine.Components
{
    /// <summary>
    /// A container component for the entity's position and rotation.
    /// </summary>
    public class TransformComponent : Component<TransformComponent>
    {
        /// <summary>
        /// The entity's position.
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// The entity's rotation.
        /// </summary>
        public Quaternion rotation;

        /// <summary>
        /// Construct a new TransformComponent with the parameters specified.
        /// </summary>
        /// <param name="position">The entity's position.</param>
        /// <param name="rotation">The entity's rotation.</param>
        public TransformComponent(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
}
