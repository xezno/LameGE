using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Common.MathUtils;
using Example.Components;
using Quincy.Components;

namespace Example.Entities
{
    public sealed class PlayerEntity : Entity<PlayerEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.User;

        public PlayerEntity()
        {
            AddComponent(new TransformComponent(new Vector3d(0, 0, 15f), Quaternion.identity, new Vector3d(1, 1, 1)));
            AddComponent(new PlayerMovementComponent());
        }
    }
}