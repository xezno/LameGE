using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
