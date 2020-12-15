using Engine.ECS.Entities;
using Engine.ECS.Observer;
using Engine.Types;
using System;
using System.Collections.Generic;

namespace Engine.ECS.Managers
{
    public class Manager<T> : IManager
    {
        public virtual IHasParent Parent { get; set; }

        public void RenderImGui() { }

        public List<IEntity> Entities { get; } = new List<IEntity>();

        // All systems should be singletons.
        private static T privateInstance;

        /// <summary>
        /// Get the single instance of the desired manager.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (privateInstance == null)
                    CreateInstance();
                return privateInstance;
            }
        }

        public static void CreateInstance()
        {
            if (privateInstance == null)
                privateInstance = Activator.CreateInstance<T>();
        }

        public virtual void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
            foreach (var entity in Entities)
            {
                entity.OnNotify(notifyType, notifyArgs);
            }
        }

        public virtual void Run() { }

        public void AddEntity(IEntity entity)
        {
            entity.Parent = this;
            Entities.Add(entity);
        }
    }
}
