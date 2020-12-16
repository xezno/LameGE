using System.Collections.Generic;

namespace Engine.ECS.Observer
{
    public static class Subject
    {
        private static List<IObserver> observers = new List<IObserver>();

        /// <summary>
        /// Add a manager to this broadcaster - this manager will receive all notifications afterwards.
        /// </summary>
        /// <param name="manager">The manager to add.</param>
        public static void AddObserver(IObserver observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Notify all attached managers (and the game instance, if applicable) of an application-wide state change.
        /// </summary>
        /// <param name="notifyType">The notification type.</param>
        /// <param name="notifyArgs">Any applicable arguments or details to carry with the notification.</param>
        public static void Notify(NotifyType notifyType, INotifyArgs notifyArgs)
        {
            foreach (var observer in observers)
            {
                observer.OnNotify(notifyType, notifyArgs);
            }
        }
    }
}
