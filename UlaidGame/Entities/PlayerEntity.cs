using ECSEngine.Assets;
using ECSEngine.Components;
using ECSEngine.Entities;
using ECSEngine.MathUtils;
using UlaidGame.Components;

namespace UlaidGame.Entities
{
    public sealed class PlayerEntity : Entity<PlayerEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.User;

        public PlayerEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, -120f, 15f), Quaternion.identity, new Vector3(1, 1, 1)));
            AddComponent(new PlayerMovementComponent());
        }
    }
}