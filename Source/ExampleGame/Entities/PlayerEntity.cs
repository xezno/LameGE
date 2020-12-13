using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Utils.MathUtils;
using ExampleGame.Components;
using Engine.Renderer.Components;

namespace ExampleGame.Entities
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