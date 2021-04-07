using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Renderer.Components;
using Engine.Utils.MathUtils;
using System.Numerics;

namespace Engine.Renderer.Entities
{
    /// <summary>
    /// An entity which represents the camera in the world.
    /// </summary>
    public sealed class CameraEntity : Entity<CameraEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.Camera;

        /// <summary>
        /// Constructs the camera entity with a transform component and a camera component.
        /// </summary>
        public CameraEntity(Vector3 position)
        {
            AddComponent(new TransformComponent(position, Quaternion.Identity, new Vector3(1, 1, 1)));
            AddComponent(new CameraComponent()
            {
                FieldOfView = 90f
            });
        }
    }
}
