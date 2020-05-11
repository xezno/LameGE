using Engine.Assets;
using Engine.Components;
using Engine.ECS.Entities;
using Engine.Entities;
using Engine.Renderer.GL.Components;
using Engine.Utils.MathUtils;
using Ulaid.Components;

namespace Ulaid.Entities
{
    public sealed class PlayerEntity : Entity<PlayerEntity>
    {
        public override string IconGlyph { get; } = FontAwesome5.User;

        public PlayerEntity()
        {
            AddComponent(new TransformComponent(new Vector3(0, 0, 15f), Quaternion.identity, new Vector3(1, 1, 1)));
            AddComponent(new PlayerMovementComponent());
        }
    }
}