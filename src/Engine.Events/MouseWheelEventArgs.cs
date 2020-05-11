using System;

namespace Engine.Events
{
    public class MouseWheelEventArgs : IEventArgs
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
        /// The amount by which the mouse scroll wheel has been turned.
        /// </summary>
        public int MouseScroll { get; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseWheelEventArgs"/>; the <see cref="TimeSent"/> will be automatically set.
        /// </summary>
        /// <param name="mouseScroll">The amount by which the mouse scroll wheel has been turned.</param>
        /// <param name="sender">The object triggering the event.</param>
        public MouseWheelEventArgs(int mouseScroll, object sender)
        {
            Sender = sender;
            TimeSent = DateTime.Now;
            MouseScroll = mouseScroll;
        }
    }
}