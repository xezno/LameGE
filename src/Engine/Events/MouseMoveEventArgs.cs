using Engine.MathUtils;
using System;

namespace Engine.Events
{
    public class MouseMoveEventArgs : IEventArgs
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
        /// The mouse cursor's new position relative to the window.
        /// </summary>
        public Vector2 MousePosition { get; }

        /// <summary>
        /// The mouse cursor's position relative to the last update.
        /// </summary>
        public Vector2 MouseDelta { get; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseMoveEventArgs"/>; the <see cref="TimeSent"/> will be automatically set.
        /// </summary>
        /// <param name="mousePosition">The cursor's new position relative to the window.</param>
        /// <param name="sender">The object triggering the event.</param>
        public MouseMoveEventArgs(Vector2 mouseDelta, Vector2 mousePosition, object sender)
        {
            Sender = sender;
            TimeSent = DateTime.Now;
            MouseDelta = mouseDelta;
            MousePosition = mousePosition;
        }
    }
}
