using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ButtonBehaviourDemo.Events;
using ButtonBehaviourDemo.Services;
using ButtonBehaviourDemo.Configurations;

// In-memory Event Bus
public class EventBus
{
    private readonly ConcurrentDictionary<Type, List<Func<IEvent, Task>>> _handlers = new();

    public void Subscribe<T>(Func<T, Task> handler) where T : IEvent
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new List<Func<IEvent, Task>>();

        _handlers[type].Add((evt) => handler((T)evt));
    }

    public async Task Publish<T>(T @event) where T : IEvent
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var handlers))
        {
            foreach (var handler in handlers)
            {
                await handler(@event);
            }
        }
    }
}



// Entry Point
class Program
{
    static async Task Main()
    {
        ButtonInterpretationServiceConfiguration buttonInterpretationServiceConf = new ButtonInterpretationServiceConfiguration
        {
            _multiPressTimeout = 500, // Example timeout in milliseconds
            _pressAndHoldTimeout = 1000 // Example timeout in milliseconds
        };

        var bus = new EventBus();
        var userService = new UserService(bus);
        var orderService = new OrderService(bus);
        var buttonService = new ButtonService(bus);
        var buttonInterpretationService = new ButtonInterpretationService(bus, buttonInterpretationServiceConf);
        buttonInterpretationService.addKeyToMonitorList('a');
        Dictionary<char, bool> buttonStateMap = new Dictionary<char, bool>();

        _ = StartPeriodicPressAndHoldMonitoringAsync(buttonInterpretationService, TimeSpan.FromMilliseconds(10)); // Start the periodic monitoring in the background

        while (true)
        {
            ConsoleKeyInfo key = Console.ReadKey(intercept: true); // true = don't display the key
            DateTime timestamp = DateTime.Now;
            char keyChar = key.KeyChar;
            switch (keyChar)
            {
                case 'q':
                    Console.WriteLine("\nExiting...");
                    return;
                default:
                    {
                        if(buttonStateMap.ContainsKey(keyChar))
                        {
                            buttonStateMap[keyChar] = !buttonStateMap[keyChar]; // Toggle state
                        }
                        else
                        {
                            buttonStateMap[keyChar] = true; // First press, set to pressed
                        }

                        ButtonStateChangedEvent.ButtonState buttonState = buttonStateMap[keyChar] ? ButtonStateChangedEvent.ButtonState.ePressed : ButtonStateChangedEvent.ButtonState.eReleased;
                        await buttonService.NotifyButtonPressed(keyChar, timestamp, buttonState);
                        break;
                    }
            }
        }
    }

    static async Task StartPeriodicPressAndHoldMonitoringAsync(ButtonInterpretationService buttonInterpretationService, TimeSpan interval, CancellationToken cancellationToken = default)
    {
        int counter = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            buttonInterpretationService.CheckForPressAndHold();
            await Task.Delay(interval, cancellationToken);
        }
    }
}
