using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.ButtonBehaviours
{
    public class ButtonBehaviourOnOffLatching : ButtonBehaviourOnOff
    {
        public override void handleButtonInterpretedEvent(Events.ButtonInterpretedEvent evt)
        {
            // Register press events and toggles the state of the parameter.
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

        public ButtonBehaviourOnOffLatching(EventBus eventbus, string parameterToControl) : base(eventbus, parameterToControl)
        {
        }
    }
}
