using System;

namespace ECSEngine.Events
{
    public class KeyboardEventArgs : IEventArgs
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
        /// The keyboard key relevant to the event.
        /// </summary>
        public int KeyboardKey { get; }

        /// <summary>
        /// Construct a new instance of <see cref="MouseWheelEventArgs"/>; the <see cref="TimeSent"/> will be automatically set.
        /// </summary>
        /// <param name="keyboardKey">The keyboard key relevant to the event.</param>
        /// <param name="sender">The object triggering the event.</param>
        public KeyboardEventArgs(int keyboardKey, object sender)
        {
            Sender = sender;
            TimeSent = DateTime.Now;
            KeyboardKey = keyboardKey;
        }
    }
}