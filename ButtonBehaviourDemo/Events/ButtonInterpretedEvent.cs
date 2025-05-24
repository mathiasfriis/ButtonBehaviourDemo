using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Events
{
    public class ButtonInterpretedEvent : IEvent
    {
        public enum ButtonEvent
        {
            eUnknown,
            ePressed,
            eReleased,
            ePressAndHold,
        }
        public char _buttonId { get; set; }
        public ButtonEvent _buttonEvent { get; set; }
        public DateTime _timeStamp { get; set; }

        public uint _numMultiPresses { get; set; } = 1; // Number of multi presses detected.

        public override bool Equals(object obj)
        {
            if (obj is not ButtonInterpretedEvent other)
                return false;

            if (other == null) return false;
            return _buttonId == other._buttonId && _buttonEvent == other._buttonEvent && _timeStamp == other._timeStamp && _numMultiPresses == other._numMultiPresses;
        }
    }
}