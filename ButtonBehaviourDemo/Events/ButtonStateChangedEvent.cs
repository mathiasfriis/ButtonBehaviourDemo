using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Events
{
    public class ButtonStateChangedEvent : IEvent
    {
        public char ButtonId { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
