using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ButtonBehaviourDemo.Events.ButtonStateChangedEvent;

namespace ButtonBehaviourDemo.Events
{
    public  class ParameterUpdateEvent : IEvent
    {
        public string _parameterName { get; set; }
        public int _parameterValue { get; set; }
    }
}
