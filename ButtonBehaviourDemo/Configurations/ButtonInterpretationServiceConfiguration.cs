using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ButtonBehaviourDemo.Time;

namespace ButtonBehaviourDemo.Configurations
{
    public class ButtonInterpretationServiceConfiguration
    {
        public UInt16 _multiPressTimeout { get; set; } = 500; // Max time between presses to be considered a multiPress.
        public UInt16 _pressAndHoldTimeout { get; set; } = 1500; // Time a button is held to be considered a press and hold.

        public ITime? _timeProvider { get; set; }
    }
}
