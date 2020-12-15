using Engine.ECS.Managers;
using System.Collections.Generic;

namespace Engine.ECS.Observer
{
    public static class Broadcast
    {
        private static List<IManager> managers = new List<IManager>();
        private static IManager gameInstance;

        /// <summary>
        /// Sets the manager's game instance; this is the object that will first receive all notifications.
        /// </summary>
        /// <param name="game"></param>
        public static void SetGame(IManager game)
        {
            gameInstance = game;
        }

        /// <summary>
        /// Add a manager to this broadcaster - this manager will receive all notifications afterwards.
        /// </summary>
        /// <param name="manager">The manager to add.</param>
        public static void AddManager(IManager manager)
        {
            managers.Add(manager);
        }

        /// <summary>
        /// Notify all attached managers (and the game instance, if applicable) of an application-wide state change.
        /// </summary>
        /// <param name="notifyType">The notification type.</param>
        /// <param name="notifyArgs">Any applicable arguments or details to carry with the notification.</param>
        public static void Notify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
            gameInstance?.OnNotify(notifyType, notifyArgs);

            foreach (var manager in managers)
            {
                manager.OnNotify(notifyType, notifyArgs);
            }
        }
    }
}
