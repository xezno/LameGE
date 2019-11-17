using System;

namespace ECSEngine.Events
{
    /// <summary>
    /// Generic event arguments for use when specific parameters are not required.
    /// </summary>
    public class GenericEventArgs : IEventArgs
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
        /// Construct a new instance of <see cref="GenericEventArgs"/>; the <see cref="timeSent"/> will be automatically set.
        /// </summary>
        /// <param name="sender">The object triggering the event.</param>
        public GenericEventArgs(object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
        }
    }
}
