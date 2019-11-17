using System;

namespace ECSEngine.Events
{
    public class KeyboardEventArgs : IEventArgs
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
        /// The keyboard key relevant to the event.
        /// </summary>
        public int keyboardKey { get; set; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseWheelEventArgs"/>; the <see cref="timeSent"/> will be automatically set.
        /// </summary>
        /// <param name="keyboardKey">The keyboard key relevant to the event.</param>
        /// <param name="sender">The object triggering the event.</param>
        public KeyboardEventArgs(int keyboardKey, object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
            this.keyboardKey = keyboardKey;
        }
    }
}