using ECSEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSEngine
{
    public interface IEntity
    {
        void HandleEvent(Event eventType, IEventArgs eventArgs);
    }
}
