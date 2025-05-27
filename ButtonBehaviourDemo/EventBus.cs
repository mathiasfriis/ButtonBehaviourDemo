using ButtonBehaviourDemo.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class EventBus
{
    // Threadsafe dictionary, that maps event types to a list of event handlers.
    private readonly ConcurrentDictionary<Type, List<Action<IEvent>>> _subscribers = new();

    public void Subscribe<T>(Action<T> handler, ActionQueue queue) where T : IEvent
    {
        var type = typeof(T);

        // If map doesn't contain the key representing the parsed event type, create a new list for it.
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Action<IEvent>>();

        // Add the new handler to the list.
        // The handler is wrapped in a lambda that enqueues the action to be processed later, by any process with a reference to the ActionQueue.
        _subscribers[type].Add(evt => queue.Enqueue(() => handler((T)evt)));
    }

    public void Publish<T>(T @event) where T : IEvent
    {
        var type = typeof(T);

        // Check if there are any subscribers for the event type.
        if (_subscribers.TryGetValue(type, out var handlers))
        {
            // If there are, invoke each handler with the event.
            // Note that this event merely enqueues the actual action to be processed later, allowing for asynchronous processing.
            // Note: Do we really need this nested lambda stuff? Isn't it possible to add to the queue directly?
            foreach (var handler in handlers)
            {
                handler(@event);
            }
        }
    }
}

// An action queue is, as the name suggest, a queue that holds actions to be processed later.
// In our design, each service will have its own ActionQueue, allowing each service to handle only its own events.
public class ActionQueue
{
    // Threadsafe queue to hold actions to be processed later.
    private readonly ConcurrentQueue<Action> _queue = new();

    // Enqueue an action to be processed later.
    public void Enqueue(Action action) => _queue.Enqueue(action);

    // Process all actions in the queue, executing them in the order they were added.
    public void Process()
    {
        while (_queue.TryDequeue(out var action))
        {
            action();
        }
    }
}
