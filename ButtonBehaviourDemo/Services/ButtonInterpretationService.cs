using ButtonBehaviourDemo.Events;
using System;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Services
{
    public class ButtonInterpretationService
    {
        private readonly EventBus _bus;
        private readonly Dictionary<char, DateTime> _buttonToTimeStampMap = new(){};

        public void addKeyToMonitorList(char keyToMonitor) // This could also be done via a configuration.
        {
            _buttonToTimeStampMap.Add(keyToMonitor, DateTime.MinValue); // Initialize with a default value that will never trigger e.g. double presses
        }
        public ButtonInterpretationService(EventBus bus)
        {
            _bus = bus;
            _bus.Subscribe<ButtonStateChangedEvent>(HandleButtonStateChanged);
        }
        public async Task NotifyButtonPressed(char key, DateTime timestamp)
        {
            await _bus.Publish(new ButtonStateChangedEvent { ButtonId = key, TimeStamp = timestamp });
        }

        private Task HandleButtonStateChanged(ButtonStateChangedEvent evt)
        {
            if(_buttonToTimeStampMap.ContainsKey(evt.ButtonId))
            {
                Console.WriteLine($"[OrderService] Detected Button press for button: " + evt.ButtonId + " at time: " + evt.TimeStamp.ToString());
            }
            else
            {
                Console.WriteLine("Key '" + evt.ButtonId + "' unknown to ButtonInterpretationService");
            }
            
            return Task.CompletedTask;
        }
    }
}
