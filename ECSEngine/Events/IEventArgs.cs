using System;

namespace ECSEngine.Events
{
    // TODO: Consider - do we actually need this? C# already provides a pretty neat EventArgs class implementation out of the box.
    public interface IEventArgs
    {
        object sender { get; set; }
        DateTime timeSent { get; set; }
    }
}
