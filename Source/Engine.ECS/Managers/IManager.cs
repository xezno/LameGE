using Engine.ECS.Entities;
using Engine.ECS.Observer;
using Engine.Types;
using System.Collections.Generic;

namespace Engine.ECS.Managers
{
    public interface IManager : IHasParent, IObserver
    {
        /// <summary>
        /// A list of entities that the manager contains.
        /// </summary>
        List<IEntity> Entities { get; }

        /// <summary>
        /// Adds an entity to this manager.
        /// </summary>
        /// <param name="entity"></param>
        void AddEntity(IEntity entity);

        /// <summary>
        /// Called whenever the manager should run its typical process. (Usually called in game loop).
        /// </summary>
        void Run();
    }
}
