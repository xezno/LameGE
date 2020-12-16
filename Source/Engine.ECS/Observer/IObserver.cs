namespace Engine.ECS.Observer
{
    public interface IObserver
    {
        /// <summary>
        /// Called when a notification is broadcast.
        /// </summary>
        /// <param name="notifyType">The type of the notification broadcast.</param>
        /// <param name="notifyArgs">Any relevant information about the notification.</param>
        void OnNotify(NotifyType notifyType, INotifyArgs notifyArgs);
    }
}
