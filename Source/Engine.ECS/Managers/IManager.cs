using Engine.ECS.Entities;
using Engine.ECS.Observer;
using Engine.Types;
using System.Collections.Generic;

namespace Engine.ECS.Managers
{
    public interface IManager : IHasParent
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
        /// Called when a notification is broadcast.
        /// </summary>
        /// <param name="notifyType">The type of the notification broadcast.</param>
        /// <param name="notifyArgs">Any relevant information about the notification.</param>
        void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs);

        /// <summary>
        /// Called whenever the manager should run its typical process. (Usually called in game loop).
        /// </summary>
        void Run();
    }
}
