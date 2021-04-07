using Engine.Assets;
using Engine.ECS.Entities;
using Engine.Renderer.Components;
using Engine.Utils;
using Engine.Utils.MathUtils;
using ExampleGame.Components;
using System.Numerics;

namespace ExampleGame.Entities
{
    public sealed class PlayerEntity : Entity<PlayerEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.User;

        public PlayerEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 0, 15f), Quaternion.Identity, new Vector3(1, 1, 1)));
            AddComponent(new PlayerMovementComponent());
        }
    }
}