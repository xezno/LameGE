using ECSEngine.MathUtils;
using System;

namespace ECSEngine.Events
{
    class WindowResizeEventArgs : IEventArgs
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
        /// The window's new size.
        /// </summary>
        public Vector2 WindowSize { get; }

        /// <summary>
        /// Construct a new instance of <see cref="WindowResizeEventArgs"/>; the <see cref="TimeSent"/> will be automatically set.
        /// </summary>
        /// <param name="windowSize">The window's new size.</param>
        /// <param name="sender">The object triggering the event.</param>
        public WindowResizeEventArgs(Vector2 windowSize, object sender)
        {
            Sender = sender;
            TimeSent = DateTime.Now;
            WindowSize = windowSize;
        }
    }
}