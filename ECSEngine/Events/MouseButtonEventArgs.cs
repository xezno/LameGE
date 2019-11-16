using System;

namespace ECSEngine.Events
{
    public class MouseButtonEventArgs : IEventArgs
    {
        public object sender { get; set; }
        public DateTime timeSent { get; set; }

        public int mouseButton { get; set; }

        public MouseButtonEventArgs(int mouseButton, object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
            this.mouseButton = mouseButton;
        }
    }
}