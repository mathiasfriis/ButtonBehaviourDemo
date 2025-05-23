using ButtonBehaviourDemo.Configurations;
using ButtonBehaviourDemo.Events;
using System;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Services
{
    public class ButtonInterpretationService
    {
        private readonly EventBus _bus;
        private readonly Dictionary<char, DateTime> _buttonToTimeStampMap = new(){}; // Map that tracks the last time each button was pressed.
        private readonly Dictionary<char, UInt16> _buttonToConsecutivePressesMap = new() { }; // Map that tracks how many consecutive presses have been detected for each button.
        ButtonInterpretationServiceConfiguration _conf;

        public void addKeyToMonitorList(char keyToMonitor) // This could also be done via a configuration.
        {
            _buttonToTimeStampMap.Add(keyToMonitor, DateTime.MinValue); // Initialize with a default value that will never trigger e.g. double presses
            _buttonToConsecutivePressesMap.Add(keyToMonitor, 0); // Initialize with a default value of 0 presses.
        }
        public ButtonInterpretationService(EventBus bus, ButtonInterpretationServiceConfiguration conf)
        {
            _conf = conf;

            _bus = bus;
            _bus.Subscribe<ButtonStateChangedEvent>(HandleButtonStateChanged);
        }
        public async Task NotifyButtonPressed(char key, DateTime timestamp)
        {
            await _bus.Publish(new ButtonStateChangedEvent { _buttonId = key, _timeStamp = timestamp });
        }

        private Task HandleButtonStateChanged(ButtonStateChangedEvent evt)
        {
            if(_buttonToTimeStampMap.ContainsKey(evt._buttonId))
            {
                Console.WriteLine($"[OrderService] Key: " + evt._buttonId + " , State: " + evt._buttonState + ", Time: " + evt._timeStamp.ToString());

                // Check for multiPress
                if(evt._buttonState == ButtonStateChangedEvent.ButtonState.ePressed)
                {
                    // Calculate time since last press
                    TimeSpan timeSinceLastPress = evt._timeStamp - _buttonToTimeStampMap[evt._buttonId];
                    if (timeSinceLastPress.TotalMilliseconds <= _conf._multiPressTimeout)
                    {
                        // MultiPress detected
                        _buttonToConsecutivePressesMap[evt._buttonId]++;
                        Console.WriteLine($"[ButtonInterpretationService] MultiPress detected for key: {evt._buttonId}, Count: {_buttonToConsecutivePressesMap[evt._buttonId]}");
                    }
                    else
                    {
                        // Reset consecutive presses count
                        _buttonToConsecutivePressesMap[evt._buttonId] = 1;
                        Console.WriteLine($"[ButtonInterpretationService] Single Press detected for key: {evt._buttonId}");
                    }
                    _buttonToTimeStampMap[evt._buttonId] = evt._timeStamp; // Update the last press time for this button.
                }
            }
            else
            {
                Console.WriteLine("Key '" + evt._buttonId + "' unknown to ButtonInterpretationService");
            }
            
            return Task.CompletedTask;
        }
    }
}
