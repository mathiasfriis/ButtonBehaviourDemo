using ButtonBehaviourDemo.Configurations;
using ButtonBehaviourDemo.Events;
using ButtonBehaviourDemo.Services;
using ButtonBehaviourDemo.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ButtonBehaviourDemo.Events.ButtonInterpretedEvent;
using static ButtonBehaviourDemo.Events.ButtonStateChangedEvent;

namespace ButtonBehaviourTest
{
    [TestFixture]
    public class ButtoBehaviourServiceTest
    {
        public static readonly char TEST_KEY = 'a'; // Example key to test
        public static readonly DateTime TIME_ZERO = DateTime.MinValue; // Example timestamp for testing
        public static readonly string PARAMETER_NAME = "testParm"; // Example parameter to control
        public static readonly string TEMPO_ID = "tempoId"; // Example parameter to control

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
            rx.Subscribe<ParameterUpdateEvent>();
            rx.Subscribe<TempoTimeStampEvent>();
        }

        private void simulateButtonPress(DateTime timeStamp)
        {
            eventBus.Publish(new ButtonInterpretedEvent { _buttonId = TEST_KEY, _timeStamp = timeStamp, _buttonEvent = ButtonInterpretedEvent.ButtonEvent.ePressed});
        }

        private void simulateButtonRelease(DateTime timeStamp)
        {
            eventBus.Publish(new ButtonInterpretedEvent { _buttonId = TEST_KEY, _timeStamp = timeStamp, _buttonEvent = ButtonInterpretedEvent.ButtonEvent.eReleased });
        }

        private void simulateButtonPressAndHold(DateTime timeStamp)
        {
            eventBus.Publish(new ButtonInterpretedEvent { _buttonId = TEST_KEY, _timeStamp = timeStamp, _buttonEvent = ButtonInterpretedEvent.ButtonEvent.ePressAndHold });
        }

        private void handleEvents(uint nTimes)
        {
            for (int i = 0; i < nTimes; i++)
            {
                buttonBehaviourService.HandleEvents();
                rx.HandleEvents();
            }
        }

        [Test]
        public void NoAction_NoMessagesSent()
        {
            Assert.That(rx.Received<ButtonInterpretedEvent>(), Is.EqualTo(0));
        }

        [Test]
        public void LatchingOnOff_CanTurnOnAndOff()
        {
            DateTime time = TIME_ZERO;

            buttonBehaviourService.SetButtonBehaviour(TEST_KEY, new ButtonBehaviourDemo.ButtonBehaviours.ButtonBehaviourOnOffLatching(eventBus, PARAMETER_NAME));

            Assert.That(rx.Received<ButtonInterpretedEvent>(), Is.EqualTo(0));

            //Simulate a regular button press, including release shortly after.
            simulateButtonPress(time);
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter on
            Assert.That(rx.Received<ParameterUpdateEvent>(), Is.EqualTo(1));
            Assert.That(rx.ReceivedSimilar<ParameterUpdateEvent>(new ParameterUpdateEvent {_parameterName = PARAMETER_NAME, _parameterValue = 1}), Is.EqualTo(1));

            //Simulate a regular button press, including release shortly after.
            time = time.AddMilliseconds(3000);
            simulateButtonPress(time);
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter off
            Assert.That(rx.Received<ParameterUpdateEvent>(), Is.EqualTo(2));
            Assert.That(rx.ReceivedSimilar<ParameterUpdateEvent>(new ParameterUpdateEvent { _parameterName = PARAMETER_NAME, _parameterValue = 0 }), Is.EqualTo(1));
        }

        [Test]
        public void MomentaryOnOff_CanTurnOnAndOff()
        {
            DateTime time = TIME_ZERO;

            buttonBehaviourService.SetButtonBehaviour(TEST_KEY, new ButtonBehaviourDemo.ButtonBehaviours.ButtonBehaviourOnOffMomentary(eventBus, PARAMETER_NAME));

            Assert.That(rx.Received<ButtonInterpretedEvent>(), Is.EqualTo(0));

            //Simulate a regular button press, but without release.
            simulateButtonPress(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter on
            Assert.That(rx.Received<ParameterUpdateEvent>(), Is.EqualTo(1));
            Assert.That(rx.ReceivedSimilar<ParameterUpdateEvent>(new ParameterUpdateEvent { _parameterName = PARAMETER_NAME, _parameterValue = 1 }), Is.EqualTo(1));

            //Simulate that we release the button after a while.
            time = time.AddMilliseconds(3000);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter off
            Assert.That(rx.Received<ParameterUpdateEvent>(), Is.EqualTo(2));
            Assert.That(rx.ReceivedSimilar<ParameterUpdateEvent>(new ParameterUpdateEvent { _parameterName = PARAMETER_NAME, _parameterValue = 0 }), Is.EqualTo(1));
        }

        [Test]
        public void TapTempo_PressesSendTempoMessages()
        {
            DateTime time = TIME_ZERO;
            DateTime timeLastPress;

            buttonBehaviourService.SetButtonBehaviour(TEST_KEY, new ButtonBehaviourDemo.ButtonBehaviours.ButtonBehaviourTapTempo(eventBus, TEMPO_ID));

            Assert.That(rx.Received<ButtonInterpretedEvent>(), Is.EqualTo(0));

            //Simulate a regular button press, including release shortly after.
            simulateButtonPress(time);
            timeLastPress = time;
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter on
            Assert.That(rx.Received<TempoTimeStampEvent>(), Is.EqualTo(1));
            Assert.That(rx.ReceivedSimilar<TempoTimeStampEvent>(new TempoTimeStampEvent { _tempoId = TEMPO_ID, _timestamp = timeLastPress }), Is.EqualTo(1));

            //Simulate a regular button press, including release shortly after.
            time = time.AddMilliseconds(3000);
            simulateButtonPress(time);
            timeLastPress = time;
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter off
            Assert.That(rx.Received<TempoTimeStampEvent>(), Is.EqualTo(2));
            Assert.That(rx.ReceivedSimilar<TempoTimeStampEvent>(new TempoTimeStampEvent { _tempoId = TEMPO_ID, _timestamp = timeLastPress }), Is.EqualTo(1));

            //Simulate a regular button press, including release shortly after.
            time = time.AddMilliseconds(3000);
            simulateButtonPress(time);
            timeLastPress = time;
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter off
            Assert.That(rx.Received<TempoTimeStampEvent>(), Is.EqualTo(3));
            Assert.That(rx.ReceivedSimilar<TempoTimeStampEvent>(new TempoTimeStampEvent { _tempoId = TEMPO_ID, _timestamp = timeLastPress }), Is.EqualTo(1));
        }

        [Test]
        public void ButtonBehaviourOnOffLatchingPlusTapTempo_TurnsEffectOnOffPrDefault()
        {
            DateTime time = TIME_ZERO;

            buttonBehaviourService.SetButtonBehaviour(TEST_KEY, new ButtonBehaviourDemo.ButtonBehaviours.ButtonBehaviourOnOffLatchingPlusTapTempo(eventBus, PARAMETER_NAME, TEMPO_ID));

            Assert.That(rx.Received<ButtonInterpretedEvent>(), Is.EqualTo(0));

            //Simulate a regular button press, including release shortly after.
            simulateButtonPress(time);
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter on
            Assert.That(rx.Received<ParameterUpdateEvent>(), Is.EqualTo(1));
            Assert.That(rx.ReceivedSimilar<ParameterUpdateEvent>(new ParameterUpdateEvent { _parameterName = PARAMETER_NAME, _parameterValue = 1 }), Is.EqualTo(1));

            //Simulate a regular button press, including release shortly after.
            time = time.AddMilliseconds(3000);
            simulateButtonPress(time);
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter off
            Assert.That(rx.Received<ParameterUpdateEvent>(), Is.EqualTo(2));
            Assert.That(rx.ReceivedSimilar<ParameterUpdateEvent>(new ParameterUpdateEvent { _parameterName = PARAMETER_NAME, _parameterValue = 0 }), Is.EqualTo(1));

            // Check that we never received any tempo messages
            Assert.That(rx.Received<TempoTimeStampEvent>(), Is.EqualTo(0));
        }

        [Test]
        public void ButtonBehaviourOnOffLatchingPlusTapTempo_CanEnterTapTempoSessionViaPressAndHold()
        {
            DateTime time = TIME_ZERO;
            DateTime timeLastPress;

            buttonBehaviourService.SetButtonBehaviour(TEST_KEY, new ButtonBehaviourDemo.ButtonBehaviours.ButtonBehaviourOnOffLatchingPlusTapTempo(eventBus, PARAMETER_NAME, TEMPO_ID));

            //Simulate a regular button press, including release shortly after.
            simulateButtonPressAndHold(time);
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            //Simulate a regular button press, including release shortly after.
            time = time.AddMilliseconds(3000);
            simulateButtonPress(time);
            timeLastPress = time;
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter off
            Assert.That(rx.Received<TempoTimeStampEvent>(), Is.EqualTo(1));
            Assert.That(rx.ReceivedSimilar<TempoTimeStampEvent>(new TempoTimeStampEvent { _tempoId = TEMPO_ID, _timestamp = timeLastPress }), Is.EqualTo(1));

            //Simulate a regular button press, including release shortly after.
            time = time.AddMilliseconds(3000);
            simulateButtonPress(time);
            timeLastPress = time;
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter off
            Assert.That(rx.Received<TempoTimeStampEvent>(), Is.EqualTo(2));
            Assert.That(rx.ReceivedSimilar<TempoTimeStampEvent>(new TempoTimeStampEvent { _tempoId = TEMPO_ID, _timestamp = timeLastPress }), Is.EqualTo(1));

            //Simulate a regular button press, including release shortly after.
            time = time.AddMilliseconds(3000);
            simulateButtonPress(time);
            timeLastPress = time;
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter off
            Assert.That(rx.Received<TempoTimeStampEvent>(), Is.EqualTo(3));
            Assert.That(rx.ReceivedSimilar<TempoTimeStampEvent>(new TempoTimeStampEvent { _tempoId = TEMPO_ID, _timestamp = timeLastPress }), Is.EqualTo(1));
        }

        [Test]
        public void ButtonBehaviourOnOffLatchingPlusTapTempo_CanExitTapTempoSessionViaPressAndHold()
        {
            DateTime time = TIME_ZERO;
            DateTime timeLastPress;

            buttonBehaviourService.SetButtonBehaviour(TEST_KEY, new ButtonBehaviourDemo.ButtonBehaviours.ButtonBehaviourOnOffLatchingPlusTapTempo(eventBus, PARAMETER_NAME, TEMPO_ID));

            //Simulate a press and hold, including release shortly after.
            simulateButtonPressAndHold(time);
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // We should now be in Tap Tempo session, so tempo events are sent instead of parameter updates.

            //Simulate a regular button press, including release shortly after.
            time = time.AddMilliseconds(3000);
            simulateButtonPress(time);
            timeLastPress = time;
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter off
            Assert.That(rx.Received<TempoTimeStampEvent>(), Is.EqualTo(1));
            Assert.That(rx.ReceivedSimilar<TempoTimeStampEvent>(new TempoTimeStampEvent { _tempoId = TEMPO_ID, _timestamp = timeLastPress }), Is.EqualTo(1));

            //Simulate a press and hold, including release shortly after.
            time = time.AddMilliseconds(3000);
            simulateButtonPressAndHold(time);
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            // We should now be in the latching on/off mode.

            //Simulate a regular button press, including release shortly after.
            time = time.AddMilliseconds(3000);
            simulateButtonPress(time);
            timeLastPress = time;
            time = time.AddMilliseconds(100);
            simulateButtonRelease(time);

            handleEvents(1);

            // Check that we receieved a message to turn the parameter on
            Assert.That(rx.Received<ParameterUpdateEvent>(), Is.EqualTo(1));
            Assert.That(rx.ReceivedSimilar<ParameterUpdateEvent>(new ParameterUpdateEvent { _parameterName = PARAMETER_NAME, _parameterValue = 1 }), Is.EqualTo(1));
        }
    }
}
