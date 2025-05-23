using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.ButtonBehaviours
{
    internal class ButtonBehaviourOnOffLatching : IButtonBehaviour
    {
        public void handleButtonInterpretedEvent(Events.ButtonInterpretedEvent evt) 
        {
            // Simple state machine that registers press events and toggles the state of the parameter.
            switch (evt._buttonEvent)
            {
                case Events.ButtonInterpretedEvent.ButtonEvent.ePressed:
                {
                    // Handle button pressed event
                    SetParameterState(!GetParameterState());
                    break;
                }
                default:
                {
                    break;
                }
            }
        }
        private void SetParameterState(bool value) {
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

        public ButtonBehaviourOnOffLatching(string parameterToControl) {
            _parameterToControl = parameterToControl;
        }


    }
}
