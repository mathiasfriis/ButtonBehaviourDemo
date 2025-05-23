using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Events
{
    public class ButtonStateChangedEvent : IEvent
    {
        public enum ButtonState
        {
            ePressed,
            eReleased
        }
        public char _buttonId { get; set; }
        public ButtonState _buttonState { get; set; }
        public DateTime _timeStamp { get; set; }
    }
}
