using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Events
{
    public class OrderPlacedEvent : IEvent
    {
        public int OrderId { get; set; }
    }
}
