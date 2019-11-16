using System;

using ECSEngine.Math;

namespace ECSEngine.Events
{
    class WindowResizeEventArgs : IEventArgs
    {
        public object sender { get; set; }
        public DateTime timeSent { get; set; }

        public Vector2 windowSize { get; set; }

        public WindowResizeEventArgs(Vector2 windowSize, object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
            this.windowSize = windowSize;
        }
    }
}