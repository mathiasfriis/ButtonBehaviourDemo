using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.ButtonBehaviours
{
    public class ButtonBehaviourTapTempo : BaseButtonBehaviour
    {
        public override void handleButtonInterpretedEvent(Events.ButtonInterpretedEvent evt)
        {
            // Simple state machine that registers press events and relays the time stamp message to part of the system that takes care of tempo management.
            switch (evt._buttonEvent)
            {
                case Events.ButtonInterpretedEvent.ButtonEvent.ePressed:
                {
                    _eventBus.Publish(new Events.TempoTimeStampEvent { _tempoId = _tempoId, _timestamp = evt._timeStamp });
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        string _tempoId;

        public ButtonBehaviourTapTempo(EventBus eventbus, string tempoId) : base(eventbus)
        {
            _tempoId = tempoId;
        }
    }
}
