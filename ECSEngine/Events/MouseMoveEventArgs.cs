using ECSEngine.MathUtils;
using System;

namespace ECSEngine.Events
{
    public class MouseMoveEventArgs : IEventArgs
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
        /// The mouse cursor's new position relative to the window.
        /// </summary>
        public Vector2 mousePosition { get; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseMoveEventArgs"/>; the <see cref="timeSent"/> will be automatically set.
        /// </summary>
        /// <param name="mousePosition">The cursor's new position relative to the window.</param>
        /// <param name="sender">The object triggering the event.</param>
        public MouseMoveEventArgs(Vector2 mousePosition, object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
            this.mousePosition = mousePosition;
        }
    }
}
