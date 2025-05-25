using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.ButtonBehaviours
{
    public class ButtonBehaviourOnOffLatchingPlusTapTempo : BaseButtonBehaviour
    {
        ButtonBehaviourOnOffLatching _buttonBehaviourOnOffLatching;
        ButtonBehaviourTapTempo _buttonBehaviourTapTempo;
        bool _isInTapTempoSession = false;
        public override void handleButtonInterpretedEvent(Events.ButtonInterpretedEvent evt)
        {
            ButtonBehaviourOnOffLatching buttonBehaviourOnOffLatching = new ButtonBehaviourOnOffLatching(_eventBus, _tempoId);
            // Simple state machine that registers press events and relays the time stamp message to part of the system that takes care of tempo management.
            switch (evt._buttonEvent)
            {
                case Events.ButtonInterpretedEvent.ButtonEvent.ePressAndHold: // Press and hold switches between the two behaviours.
                    {
                        if (_isInTapTempoSession)
                        {
                            Console.WriteLine("Exiting Tap Tempo session, switching to On/Off Latching behaviour");
                            _isInTapTempoSession = false;
                        }
                        else
                        {
                            Console.WriteLine("Entering Tap Tempo session, switching to Tap Tempo behaviour");
                            _isInTapTempoSession = true;
                        }
                        break;
                    }
                case Events.ButtonInterpretedEvent.ButtonEvent.ePressed:
                    {
                        if (_isInTapTempoSession)
                        {
                            _buttonBehaviourTapTempo.handleButtonInterpretedEvent(evt);
                        }
                        else
                        {
                            _buttonBehaviourOnOffLatching.handleButtonInterpretedEvent(evt);
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        string _tempoId;

        public ButtonBehaviourOnOffLatchingPlusTapTempo(EventBus eventbus, string parameterId, string tempoId) : base(eventbus)
        {
            _tempoId = tempoId;
            _buttonBehaviourTapTempo = new ButtonBehaviourTapTempo(eventbus, tempoId);
            _buttonBehaviourOnOffLatching = new ButtonBehaviourOnOffLatching(eventbus, parameterId);
        }
    }
}
