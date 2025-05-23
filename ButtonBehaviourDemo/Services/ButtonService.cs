using ButtonBehaviourDemo.Events;
using System;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Services
{
    public class ButtonService : BaseService
    {
        public ButtonService(EventBus bus) : base(bus)
        {
        }
        public void NotifyButtonPressed(char key, DateTime timestamp, ButtonStateChangedEvent.ButtonState buttonState)
        {
            Publish(new ButtonStateChangedEvent { _buttonId = key, _timeStamp = timestamp, _buttonState = buttonState});
        }

       
    }
}
