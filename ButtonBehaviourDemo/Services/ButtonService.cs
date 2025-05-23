using ButtonBehaviourDemo.Events;
using System;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Services
{
    public class ButtonService
    {
        private readonly EventBus _bus;
        public ButtonService(EventBus bus)
        {
            _bus = bus;
        }
        public async Task NotifyButtonPressed(char key, DateTime timestamp, ButtonStateChangedEvent.ButtonState buttonState)
        {
            await _bus.Publish(new ButtonStateChangedEvent { _buttonId = key, _timeStamp = timestamp, _buttonState = buttonState});
        }

       
    }
}
