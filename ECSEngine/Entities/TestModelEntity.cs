using ECSEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECSEngine.Entities
{
    public class TestModelEntity : IEntity
    {
        public TestModelEntity() { }
        public void HandleEvent(Event eventType, IEventArgs eventArgs)
        {
            Debug.Log($"Received event {eventType.ToString()}");
        }
    }
}
