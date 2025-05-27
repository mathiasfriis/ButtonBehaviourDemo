using ButtonBehaviourDemo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ButtonBehaviourDemo.Events.ButtonStateChangedEvent;

namespace ButtonBehaviourDemo.ButtonBehaviours
{
    public class ButtonBehaviourOnOffMomentary : ButtonBehaviourOnOff
    {
        public override void handleButtonInterpretedEvent(Events.ButtonInterpretedEvent evt)
        {
            // Press enables the parameter, release disables it.
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
                    // Handle button pressed event
                    SetParameterState(false);
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        public ButtonBehaviourOnOffMomentary(EventBus eventbus, string parameterToControl) : base(eventbus, parameterToControl)
        {
        }


    }
}
