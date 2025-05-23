using ButtonBehaviourDemo.Configurations;
using ButtonBehaviourDemo.Events;
using System;
using System.Threading.Tasks;
using ButtonBehaviourDemo.ButtonBehaviours;

namespace ButtonBehaviourDemo.Services
{
    public class ButtonBehaviourService : BaseService
    {
        ButtonBehaviourOnOffMomentary buttonBehaviourOnOffLatching = new ButtonBehaviourOnOffMomentary("testParm");
        ButtonBehaviourServiceConfiguration _conf;

        public ButtonBehaviourService(EventBus bus, ButtonBehaviourServiceConfiguration conf) : base(bus)
        {
            _conf = conf;

            Subscribe<ButtonInterpretedEvent>(HandleButtonInterpretedEvent);
        }
        private void HandleButtonInterpretedEvent(ButtonInterpretedEvent evt)
        {
            Console.WriteLine($"[ButtonBehaviourService] Interpreted event for key: {evt._buttonId}, Event: {evt._buttonEvent}, Time: {evt._timeStamp}");
            buttonBehaviourOnOffLatching.handleButtonInterpretedEvent(evt);
        }
    }
}
