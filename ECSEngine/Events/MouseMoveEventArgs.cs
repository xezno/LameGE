using System;

using ECSEngine.Math;

namespace ECSEngine.Events
{
    class MouseMoveEventArgs : IEventArgs
    {
        public object sender { get; set; }
        public DateTime timeSent { get; set; }

        public Vector2 mousePosition { get; set; }

        public MouseMoveEventArgs(Vector2 mousePosition, object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
            this.mousePosition = mousePosition;
        }
    }
}
