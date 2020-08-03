using Engine.Assets;
using Engine.Components;
using Engine.ECS.Entities;
using Engine.Renderer.GL.Components;
using Engine.Utils.MathUtils;
using OpenGL;
using Quaternion = Engine.Utils.MathUtils.Quaternion;

namespace Engine.Renderer.GL.Entities
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
        public CameraEntity(int gameResX, int gameResY)
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, 0f), Quaternion.identity, new Vector3(1, 1, 1)));
            AddComponent(new CameraComponent());
        }
    }
}
