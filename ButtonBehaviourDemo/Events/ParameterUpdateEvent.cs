using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ButtonBehaviourDemo.Events.ButtonInterpretedEvent;
using static ButtonBehaviourDemo.Events.ButtonStateChangedEvent;

namespace ButtonBehaviourDemo.Events
{
    public  class ParameterUpdateEvent : IEvent
    {
        public string _parameterName { get; set; }
        public int _parameterValue { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not ParameterUpdateEvent other)
                return false;

            if (other == null) return false;
            return _parameterName == other._parameterName && _parameterValue == other._parameterValue;
        }
    }     
}
