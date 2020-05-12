using System;

namespace Engine.Events
{
    public class MouseButtonEventArgs : IEventArgs
    {
        /// <summary>
        /// The object triggering the event.
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// The time at which the event was triggered.
        /// </summary>
        public DateTime TimeSent { get; set; }

        /// <summary>
        /// The mouse button relevant to the event.
        /// </summary>
        public int MouseButton { get; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseButtonEventArgs"/>; the <see cref="TimeSent"/> will be automatically set.
        /// </summary>
        /// <param name="mouseButton">The mouse button relevant to the event.</param>
        /// <param name="sender">The object triggering the event.</param>
        public MouseButtonEventArgs(int mouseButton, object sender)
        {
            Sender = sender;
            TimeSent = DateTime.Now;
            MouseButton = mouseButton;
        }
    }
}