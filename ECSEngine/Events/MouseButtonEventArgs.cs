using System;

namespace ECSEngine.Events
{
    public class MouseButtonEventArgs : IEventArgs
    {
        /// <summary>
        /// The object triggering the event.
        /// </summary>
        public object sender { get; set; }

        /// <summary>
        /// The time at which the event was triggered.
        /// </summary>
        public DateTime timeSent { get; set; }

        /// <summary>
        /// The mouse button relevant to the event.
        /// </summary>
        public int mouseButton { get; set; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseButtonEventArgs"/>; the <see cref="timeSent"/> will be automatically set.
        /// </summary>
        /// <param name="mouseButton">The mouse button relevant to the event.</param>
        /// <param name="sender">The object triggering the event.</param>
        public MouseButtonEventArgs(int mouseButton, object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
            this.mouseButton = mouseButton;
        }
    }
}