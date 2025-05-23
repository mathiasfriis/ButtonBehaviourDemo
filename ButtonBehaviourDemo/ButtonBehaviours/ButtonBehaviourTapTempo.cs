using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.ButtonBehaviours
{
    internal class ButtonBehaviourTapTempo : IButtonBehaviour
    {
        public void handleButtonInterpretedEvent(Events.ButtonInterpretedEvent evt)
        {
            // Simple state machine that registers press events and relays the time stamp message to part of the system that takes care of tempo management.
            switch (evt._buttonEvent)
            {
                case Events.ButtonInterpretedEvent.ButtonEvent.ePressed:
                {
                    Console.WriteLine("Sending TimeStamp message to TempoManagementService");
                    break;
                }
                default:
                {
                    break;
                }
            }
        }
    }
}
