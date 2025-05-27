using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ButtonBehaviourDemo.Events;

// Service used in tests.
// Service can subscribe to multiple event types and collect them in queues.
// Tester can then check how many events of each type have been received, and how many of them are similar to a provided example event.
public class MultiEventCollectorService : BaseService
{
    // Dictionary: event type => queue of events of that type
    private readonly ConcurrentDictionary<Type, object> _queues = new();

    public MultiEventCollectorService(EventBus bus) : base(bus)
    {
    }

    // Subscribe to a new event type T
    public void Subscribe<T>() where T : IEvent
    {
        // Create a queue for this event type if not exists
        _queues.GetOrAdd(typeof(T), _ => new ConcurrentQueue<T>());

        // Subscribe handler to event bus
        base.Subscribe<T>(evt =>
        {
            var queueObj = _queues[typeof(T)];
            var queue = (ConcurrentQueue<T>)queueObj;
            queue.Enqueue(evt);
        });
    }

    // Get the number of received events of type T
    public int Received<T>() where T : IEvent
    {
        if (_queues.TryGetValue(typeof(T), out var queueObj))
        {
            var queue = (ConcurrentQueue<T>)queueObj;
            return queue.Count;
        }
        return 0;
    }

    // Get the number of received events of type T that are similar to the provided example
    public int ReceivedSimilar<T>(T example) where T : IEvent
    {
        if (_queues.TryGetValue(typeof(T), out var queueObj))
        {
            var queue = (ConcurrentQueue<T>)queueObj;
            int count = 0;
            foreach (var evt in queue)
            {
                if (evt.Equals(example)) // or do some default comparison here
                    count++;
            }
            return count;
        }
        return 0;
    }



    // Optionally override the periodic Service hook for any processing
    public override void Service()
    {
        // Example: you could process or log collected events here periodically
    }
}
