using ButtonBehaviourDemo.Configurations;
using ButtonBehaviourDemo.Events;
using System;
using System.Threading.Tasks;
using ButtonBehaviourDemo.ButtonBehaviours;

namespace ButtonBehaviourDemo.Services
{
    public class ButtonBehaviourService : BaseService
    {
        ButtonBehaviourServiceConfiguration _conf;
        Dictionary<char, BaseButtonBehaviour> _buttonBehaviours = new Dictionary<char, BaseButtonBehaviour>();

        public ButtonBehaviourService(EventBus bus, ButtonBehaviourServiceConfiguration conf) : base(bus)
        {
            _conf = conf;

            Subscribe<ButtonInterpretedEvent>(HandleButtonInterpretedEvent);
        }
        private void HandleButtonInterpretedEvent(ButtonInterpretedEvent evt)
        {
            if (_buttonBehaviours.TryGetValue(evt._buttonId, out var buttonBehaviour))
            {
                buttonBehaviour.handleButtonInterpretedEvent(evt);
            }
            else
            {
                Console.WriteLine($"[ButtonBehaviourService] No behaviour found for button {evt._buttonId}");
            }
        }

        public void SetButtonBehaviour(char buttonId, BaseButtonBehaviour buttonBehaviour)
        {
            _buttonBehaviours[buttonId] = buttonBehaviour;
        }
    }
}
