using System;

namespace ECSEngine.Events
{
    public interface IEventArgs
    {
        object sender { get; set; }

        DateTime timeSent { get; set; }
    }
}
