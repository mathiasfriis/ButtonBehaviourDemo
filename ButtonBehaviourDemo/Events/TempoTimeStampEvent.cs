using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ButtonBehaviourDemo.Events.ButtonStateChangedEvent;

namespace ButtonBehaviourDemo.Events
{
    public class TempoTimeStampEvent : IEvent
    {
        public string _tempoId { get; set; }
        public DateTime _timestamp { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not TempoTimeStampEvent other)
                return false;

            if (other == null) return false;
            return _tempoId == other._tempoId && _timestamp == other._timestamp;
        }
    }
}
