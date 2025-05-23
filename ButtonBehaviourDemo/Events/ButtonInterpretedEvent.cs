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

        public uint _numMultiPresses { get; set; } = 0; // Number of multi presses detected.
    }
}