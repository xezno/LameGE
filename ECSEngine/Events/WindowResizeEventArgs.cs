using ECSEngine.Math;
using System;

namespace ECSEngine.Events
{
    class WindowResizeEventArgs : IEventArgs
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
        /// The window's new size.
        /// </summary>
        public Vector2 windowSize { get; }

        /// <summary>
        /// Construct a new instance of <see cref="WindowResizeEventArgs"/>; the <see cref="timeSent"/> will be automatically set.
        /// </summary>
        /// <param name="windowSize">The window's new size.</param>
        /// <param name="sender">The object triggering the event.</param>
        public WindowResizeEventArgs(Vector2 windowSize, object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
            this.windowSize = windowSize;
        }
    }
}