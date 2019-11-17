using System;

using ECSEngine.Math;

namespace ECSEngine.Events
{
    class MouseWheelEventArgs : IEventArgs
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
        /// The amount by which the mouse scroll wheel has been turned.
        /// </summary>
        public int mouseScroll { get; set; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseWheelEventArgs"/>; the <see cref="timeSent"/> will be automatically set.
        /// </summary>
        /// <param name="mouseScroll">The amount by which the mouse scroll wheel has been turned.</param>
        /// <param name="sender">The object triggering the event.</param>
        public MouseWheelEventArgs(int mouseScroll, object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
            this.mouseScroll = mouseScroll;
        }
    }
}