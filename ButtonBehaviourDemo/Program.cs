using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ButtonBehaviourDemo.Events;
using ButtonBehaviourDemo.Services;
using ButtonBehaviourDemo.Configurations;
using ButtonBehaviourDemo.Time;
using ButtonBehaviourDemo.ButtonBehaviours;

// Entry Point
class Program
{
    static async Task Main()
    {
        ButtonInterpretationServiceConfiguration buttonInterpretationServiceConf = new ButtonInterpretationServiceConfiguration
        {
            _multiPressTimeout = 500, // Example timeout in milliseconds
            _pressAndHoldTimeout = 1000, // Example timeout in milliseconds
            _timeProvider = new TimeSystem() // Using a system time provider for real-time functionality
        };

        ButtonBehaviourServiceConfiguration buttonBehaviourServiceConfiguration = new ButtonBehaviourServiceConfiguration
        {
        };

        var bus = new EventBus();
        var buttonService = new ButtonService(bus);
        var buttonInterpretationService = new ButtonInterpretationService(bus, buttonInterpretationServiceConf);
        var buttonBehaviourService = new ButtonBehaviourService(bus, buttonBehaviourServiceConfiguration);
        buttonInterpretationService.addKeyToMonitorList('a');
        Dictionary<char, bool> buttonStateMap = new Dictionary<char, bool>();

        buttonBehaviourService.SetButtonBehaviour('a', new ButtonBehaviourOnOffLatching(bus, "testParm")); // Set a specific behaviour for the 'a' button

        buttonService.Start(10); // Start the button service with a 10ms interval
        buttonInterpretationService.Start(10); // Start the button interpretation service with a 10ms interval
        buttonBehaviourService.Start(10); // Start the button behaviour service with a 10ms interval

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
                case 'm':
                    buttonBehaviourService.SetButtonBehaviour('a', new ButtonBehaviourOnOffMomentary(bus, "testParm")); // Set a specific behaviour for the 'a' button
                    break;
                case 'l':
                    buttonBehaviourService.SetButtonBehaviour('a', new ButtonBehaviourOnOffLatching(bus, "testParm")); // Set a specific behaviour for the 'a' button
                    break;
                case 't':
                    buttonBehaviourService.SetButtonBehaviour('a', new ButtonBehaviourTapTempo(bus, "myTempoId")); // Set a specific behaviour for the 'a' button
                    break;
                case 'c': // c = composite
                    buttonBehaviourService.SetButtonBehaviour('a', new ButtonBehaviourOnOffLatchingPlusTapTempo(bus, "testParm", "myTempoId")); // Set a specific behaviour for the 'a' button
                    break;
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
                        buttonService.NotifyButtonPressed(keyChar, timestamp, buttonState);
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
