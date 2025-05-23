using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.ButtonBehaviours
{
    internal class ButtonBehaviourOnOffMomentary : IButtonBehaviour
    {
        public void handleButtonInterpretedEvent(Events.ButtonInterpretedEvent evt)
        {
            // Simple state machine that sets the parameter to true on press and false on release.
            switch (evt._buttonEvent)
            {
                case Events.ButtonInterpretedEvent.ButtonEvent.ePressed:
                {
                    // Handle button pressed event
                    SetParameterState(true);
                    break;
                }
                case Events.ButtonInterpretedEvent.ButtonEvent.eReleased:
                {
                    // Handle button released event
                    SetParameterState(false);
                    break;
                }
                default:
                {
                    break;
                }
            }
        }
        private void SetParameterState(bool value)
        {
            // Logic to set the parameter value
            Console.WriteLine($"Setting {_parameterToControl} to {value}");
            _isOn = value;
        }

        private bool GetParameterState()
        {
            return _isOn;
        }

        private string _parameterToControl;
        private bool _isOn = false;

        public ButtonBehaviourOnOffMomentary(string parameterToControl)
        {
            _parameterToControl = parameterToControl;
        }


    }
}
