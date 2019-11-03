using System;

namespace ECSEngine.Events
{
    public class GenericEventArgs : IEventArgs
    {
        public object sender { get; set; }
        public DateTime timeSent { get; set; }

        public GenericEventArgs(object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
        }
    }
}
