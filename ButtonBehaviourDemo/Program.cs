using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ButtonBehaviourDemo.Events;
using ButtonBehaviourDemo.Services;

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
        var bus = new EventBus();
        var userService = new UserService(bus);
        var orderService = new OrderService(bus);


        while(true)
        {
            var userName = Console.ReadLine();
            if(string.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Bye!");
                break;
            }
            else
            {
                await userService.CreateUser(userName);
                await Task.Delay(100); // Allow async messages to process
            }
        }
    }
}
