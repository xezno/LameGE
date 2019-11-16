using System;

using ECSEngine.Math;

namespace ECSEngine.Events
{
    class MouseWheelEventArgs : IEventArgs
    {
        public object sender { get; set; }
        public DateTime timeSent { get; set; }

        public int mouseScroll { get; set; }

        public MouseWheelEventArgs(int mouseScroll, object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
            this.mouseScroll = mouseScroll;
        }
    }
}