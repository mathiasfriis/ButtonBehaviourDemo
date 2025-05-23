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
        public async Task NotifyButtonPressed(char key, DateTime timestamp)
        {
            await _bus.Publish(new ButtonStateChangedEvent { ButtonId = key, TimeStamp = timestamp});
        }

       
    }
}
