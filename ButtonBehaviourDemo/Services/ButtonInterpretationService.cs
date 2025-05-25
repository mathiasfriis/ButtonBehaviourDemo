using ButtonBehaviourDemo.Configurations;
using ButtonBehaviourDemo.Events;
using System;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Services
{
    public class ButtonInterpretationService : BaseService
    {
        private readonly Dictionary<char, ButtonStateChangedEvent.ButtonState> _buttonStateMap = new(); // Map that tracks the state of each button.
        private readonly Dictionary<char, DateTime> _buttonToLastPressTimeMap = new(); // Map that tracks the last time each button was pressed.
        private readonly Dictionary<char, UInt16> _buttonToConsecutivePressesMap = new(); // Map that tracks how many consecutive presses have been detected for each button.
        private readonly Dictionary<char, bool> _buttonToPressAndHoldNotifiedMap = new(); // Map that tracks if a press and hold notification has been sent for each button.
        public ButtonInterpretationServiceConfiguration _conf;

        public void addKeyToMonitorList(char keyToMonitor) // This could also be done via a configuration.
        {
            _buttonToLastPressTimeMap.Add(keyToMonitor, DateTime.MinValue); // Initialize with a default value that will never trigger e.g. double presses
            _buttonToConsecutivePressesMap.Add(keyToMonitor, 0); // Initialize with a default value of 0 presses.
            _buttonStateMap.Add(keyToMonitor, ButtonStateChangedEvent.ButtonState.eUnknown); // Initialize with a default value of unknown state.
            _buttonToPressAndHoldNotifiedMap.Add(keyToMonitor, false); // Initialize with a default value of false.
        }
        public ButtonInterpretationService(EventBus bus, ButtonInterpretationServiceConfiguration conf) : base(bus)
        {
            _conf = conf;
            Subscribe<ButtonStateChangedEvent>(HandleButtonStateChanged);
        }

        private void NotifyInterpretedButtonEvent(char key, DateTime timestamp, ButtonInterpretedEvent.ButtonEvent buttonEvent, uint numMultiPresses = 0)
        {
            Console.WriteLine($"[ButtonInterpretationService] Interpreted event for key: {key}, Event: {buttonEvent}, Time: {timestamp}, MultiPresses: {numMultiPresses}");
            Publish(new ButtonInterpretedEvent { _buttonId = key, _timeStamp = timestamp, _buttonEvent = buttonEvent});
        }

        public async void CheckForPressAndHold() // Getting the time should be done through an interface, allowing mocking in unit tests.
        {
            var timeNow = _conf._timeProvider.GetTime(); // Get the current time from the configured time provider.
            foreach (var buttonState in _buttonStateMap)
            {
                char buttonId = buttonState.Key;
                // For any pressed button, check if the time since last press is greater than the configured timeout.
                // However, we need to ensure that we only check for press and hold if the button is not already in a press and hold state.
                if (buttonState.Value == ButtonStateChangedEvent.ButtonState.ePressed)
                {
                    if (_buttonToPressAndHoldNotifiedMap[buttonId] == false)
                    {
                        TimeSpan timeDiff = timeNow - _buttonToLastPressTimeMap[buttonId];
                        if (timeDiff.TotalMilliseconds >= _conf._pressAndHoldTimeout)
                        {
                            // Press and hold detected, notify the system.
                            _buttonToPressAndHoldNotifiedMap[buttonId] = true;
                            NotifyInterpretedButtonEvent(buttonId, timeNow, ButtonInterpretedEvent.ButtonEvent.ePressAndHold);
                        }
                    }
                }
            }
        }

        public override void Service() 
        {
            CheckForPressAndHold();
        }
        private void HandleButtonStateChanged(ButtonStateChangedEvent evt)
        {
            _buttonStateMap[evt._buttonId] = evt._buttonState;

            if (_buttonToLastPressTimeMap.ContainsKey(evt._buttonId))
            {
                //Console.WriteLine($"[ButtonInterpretationService] Key: " + evt._buttonId + " , State: " + evt._buttonState + ", Time: " + evt._timeStamp.ToString());

                // Check for multiPress
                switch(evt._buttonState)
                {
                    case ButtonStateChangedEvent.ButtonState.ePressed:
                    {
                        // Calculate time since last press
                        TimeSpan timeSinceLastPress = evt._timeStamp - _buttonToLastPressTimeMap[evt._buttonId];
                        if (timeSinceLastPress.TotalMilliseconds <= _conf._multiPressTimeout)
                        {
                            // MultiPress detected
                            _buttonToConsecutivePressesMap[evt._buttonId]++;
                            NotifyInterpretedButtonEvent(evt._buttonId, evt._timeStamp, ButtonInterpretedEvent.ButtonEvent.ePressed, _buttonToConsecutivePressesMap[evt._buttonId]);
                            }
                        else
                        {
                            // Reset consecutive presses count
                            _buttonToConsecutivePressesMap[evt._buttonId] = 1;
                            NotifyInterpretedButtonEvent(evt._buttonId, evt._timeStamp, ButtonInterpretedEvent.ButtonEvent.ePressed, _buttonToConsecutivePressesMap[evt._buttonId]);
                            }
                        _buttonToLastPressTimeMap[evt._buttonId] = evt._timeStamp; // Update the last press time for this button.
                        break;
                    }
                        case ButtonStateChangedEvent.ButtonState.eReleased:
                    {
                        // Reset the press and hold notification for this button when it is released.
                        _buttonToPressAndHoldNotifiedMap[evt._buttonId] = false;
                        NotifyInterpretedButtonEvent(evt._buttonId, evt._timeStamp, ButtonInterpretedEvent.ButtonEvent.eReleased);
                        break;
                        }
                    default:
                    {
                        throw new InvalidOperationException("Unknown button state: " + evt._buttonState);
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Key '" + evt._buttonId + "' unknown to ButtonInterpretationService");
            }
        }
    }
}
