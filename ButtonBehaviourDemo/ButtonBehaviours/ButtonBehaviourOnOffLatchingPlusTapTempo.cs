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
            // Press and hold switches between latching on/off behaviour and tap tempo behaviour (see documentation for those)
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
                // Relay relevant events to the appropriate behaviour based on the current session state.
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
