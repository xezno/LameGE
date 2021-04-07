using Engine.ECS.Observer;
using Engine.Types;

namespace Engine.ECS.Components
{
    /// <summary>
    /// The base interface for any component running in the engine.
    /// </summary>
    public interface IComponent : IHasParent, IObserver
    {
        /// <summary>
        /// Called whenever the engine renders a single frame.
        /// </summary>
        void Render();

        /// <summary>
        /// Called whenever the engine wishes to update all systems/entities/components.
        /// </summary>
        void Update(float deltaTime);
    }
}
