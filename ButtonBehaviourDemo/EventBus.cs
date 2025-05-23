using ButtonBehaviourDemo.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class EventBus
{
    private readonly ConcurrentDictionary<Type, List<Action<IEvent>>> _subscribers = new();

    public void Subscribe<T>(Action<T> handler, ActionQueue queue) where T : IEvent
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new List<Action<IEvent>>();

        _subscribers[type].Add(evt => queue.Enqueue(() => handler((T)evt)));
    }

    public void Publish<T>(T @event) where T : IEvent
    {
        var type = typeof(T);
        if (_subscribers.TryGetValue(type, out var handlers))
        {
            foreach (var handler in handlers)
            {
                handler(@event);
            }
        }
    }
}

public class ActionQueue
{
    private readonly ConcurrentQueue<Action> _queue = new();

    public void Enqueue(Action action) => _queue.Enqueue(action);

    public void Process()
    {
        while (_queue.TryDequeue(out var action))
        {
            action();
        }
    }
}
