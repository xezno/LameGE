using System;

namespace ECSEngine.Events
{
    public class KeyboardEventArgs : IEventArgs
    {
        public object sender { get; set; }
        public DateTime timeSent { get; set; }

        public int keyboardKey { get; set; }

        public KeyboardEventArgs(int keyboardKey, object sender)
        {
            this.sender = sender;
            this.timeSent = DateTime.Now;
            this.keyboardKey = keyboardKey;
        }
    }
}