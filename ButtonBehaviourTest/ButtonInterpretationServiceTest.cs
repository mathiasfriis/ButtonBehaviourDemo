using ButtonBehaviourDemo.Configurations;
using ButtonBehaviourDemo.Events;
using ButtonBehaviourDemo.Services;
using NUnit.Framework;
using ButtonBehaviourDemo.Events;
using ButtonBehaviourDemo.Time;
using static ButtonBehaviourDemo.Events.ButtonStateChangedEvent;
using static ButtonBehaviourDemo.Events.ButtonInterpretedEvent;

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
        TimeMock timeMock;

        [SetUp]
        public void Setup()
        {   
            timeMock = new TimeMock(); // Mock time provider for testing
            ButtonInterpretationServiceConfiguration buttonInterpretationServiceConf = new ButtonInterpretationServiceConfiguration
            {
                _multiPressTimeout = MULTIPRESS_TIMEOUT, // Example timeout in milliseconds
                _pressAndHoldTimeout = PRESS_AND_HOLD_TIMEOUT, // Example timeout in milliseconds
                _timeProvider = timeMock
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
            for(int i = 0 ; i < nTimes; i++)
            {
                buttonInterpretationService.HandleEvents();
                rx.HandleEvents();
            }
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

        [Test]
        public void PressAndHold_PressAndHoldMessageSent()
        {
            DateTime currentTime = TIME_ZERO;
            timeMock.SetTime(currentTime);
            simulateButtonPress(TIME_ZERO);

            handleEvents(5);

            currentTime = currentTime.AddMilliseconds(PRESS_AND_HOLD_TIMEOUT); // Simulate time passing
            timeMock.SetTime(currentTime);

            buttonInterpretationService.Service();

            handleEvents(5);

            Assert.That(rx.Received<ButtonInterpretedEvent>(), Is.EqualTo(2)); // Button press + Press and hold
            Assert.That(rx.ReceivedSimilar<ButtonInterpretedEvent>(new ButtonInterpretedEvent { _buttonId = TEST_KEY, _timeStamp = TIME_ZERO, _buttonEvent = ButtonEvent.ePressed, _numMultiPresses = 1 }), Is.EqualTo(1));
            Assert.That(rx.ReceivedSimilar<ButtonInterpretedEvent>(new ButtonInterpretedEvent { _buttonId = TEST_KEY, _timeStamp = currentTime, _buttonEvent = ButtonEvent.ePressAndHold, _numMultiPresses = 1 }), Is.EqualTo(1));
        }
    }
}
