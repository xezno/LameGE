using ECSEngine.Components;
using ECSEngine.Entities;
using ECSEngine.MathUtils;
using SpaceGame.Components;

namespace SpaceGame.Entities
{
    public sealed class PlayerEntity : Entity<PlayerEntity>
    {
        public PlayerEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 2f, 0f), Quaternion.identity, new Vector3(1, 1, 1)));
            // AddComponent(new CameraComponent());
            AddComponent(new PlayerMovementComponent());
        }
    }
}