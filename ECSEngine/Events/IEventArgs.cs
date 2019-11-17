using System;

namespace ECSEngine.Events
{
    // TODO: Consider - C# already provides a pretty neat EventArgs class implementation out of the box.
    /// <summary>
    /// An interface from which all event arguments must derive.
    /// </summary>
    public interface IEventArgs
    {
        /// <summary>
        /// The object triggering the event.
        /// </summary>
        object sender { get; set; }

        /// <summary>
        /// The time at which the event was triggered.
        /// </summary>
        DateTime timeSent { get; set; }
    }
}
