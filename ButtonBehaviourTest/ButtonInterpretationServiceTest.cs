using ButtonBehaviourDemo.Configurations;
using ButtonBehaviourDemo.Events;
using ButtonBehaviourDemo.Services;
using NUnit.Framework;
using static ButtonBehaviourDemo.Events.ButtonInterpretedEvent;
using static ButtonBehaviourDemo.Events.ButtonStateChangedEvent;

namespace ButtonBehaviourTest
{
    [TestFixture]
    public class ButtonInterpretationServiceTest
    {
        public static readonly UInt16 MULTIPRESS_TIMEOUT = 500; // Example timeout in milliseconds
        public static readonly UInt16 PRESS_AND_HOLD_TIMEOUT = 500; // Example timeout in milliseconds
        public static readonly char TEST_KEY = 'a'; // Example key to test
        public static readonly DateTime TIME_ZERO = DateTime.MinValue; // Example timestamp for testing

        ButtonInterpretationService buttonInterpretationService;
        MultiEventCollectorService rx; // rx = receiver
        EventBus eventBus;
        [SetUp]
        public void Setup()
        {   
            ButtonInterpretationServiceConfiguration buttonInterpretationServiceConf = new ButtonInterpretationServiceConfiguration
            {
                _multiPressTimeout = MULTIPRESS_TIMEOUT, // Example timeout in milliseconds
                _pressAndHoldTimeout = PRESS_AND_HOLD_TIMEOUT // Example timeout in milliseconds
            };

            eventBus = new EventBus();
            buttonInterpretationService = new ButtonInterpretationService(eventBus, buttonInterpretationServiceConf);
            buttonInterpretationService.addKeyToMonitorList(TEST_KEY); // Add the key to monitor
            rx = new MultiEventCollectorService(eventBus); // rx = receiver

            rx.Subscribe<ButtonInterpretedEvent>();


        }

        private void simulateButtonPress(DateTime timeStamp)
        {
            eventBus.Publish(new ButtonStateChangedEvent { _buttonId = TEST_KEY, _timeStamp = timeStamp, _buttonState = ButtonState.ePressed });
        }

        private void simulateButtonRelease(DateTime timeStamp)
        {
            eventBus.Publish(new ButtonStateChangedEvent { _buttonId = TEST_KEY, _timeStamp = timeStamp, _buttonState = ButtonState.eReleased });
        }

        private void handleEvents(uint nTimes)
        {
            buttonInterpretationService.HandleEvents();
            rx.HandleEvents();
        }

        [Test]
        public void NoAction_NoMessagesSent()
        {
            Assert.That(rx.Received<ButtonInterpretedEvent>(), Is.EqualTo(0));
        }

        [Test]
        public void SinglePress_SinglePressMessageSent()
        {
            simulateButtonPress(TIME_ZERO);

            handleEvents(5);

            Assert.That(rx.Received<ButtonInterpretedEvent>(), Is.EqualTo(1));
            Assert.That(rx.ReceivedSimilar<ButtonInterpretedEvent>(new ButtonInterpretedEvent { _buttonId = TEST_KEY, _timeStamp = TIME_ZERO, _buttonEvent = ButtonEvent.ePressed, _numMultiPresses = 1 }), Is.EqualTo(1));
        }
    }
}
