using ButtonBehaviourDemo.Configurations;
using ButtonBehaviourDemo.Events;
using ButtonBehaviourDemo.Services;
using ButtonBehaviourDemo.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonBehaviourTest
{
    [TestFixture]
    public class ButtoBehaviourServiceTest
    {
        ButtonBehaviourService buttonBehaviourService;
        MultiEventCollectorService rx; // rx = receiver
        EventBus eventBus;

        [SetUp]
        public void Setup()
        {
            ButtonBehaviourServiceConfiguration buttonBehaviourServiceConf = new ButtonBehaviourServiceConfiguration
            {
            };

            eventBus = new EventBus();
            buttonBehaviourService = new ButtonBehaviourService(eventBus, buttonBehaviourServiceConf);
            rx = new MultiEventCollectorService(eventBus); // rx = receiver

            rx.Subscribe<ButtonInterpretedEvent>();
        }
    }
}
