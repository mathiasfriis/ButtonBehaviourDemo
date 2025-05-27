using ButtonBehaviourDemo.Events;
using ButtonBehaviourDemo.Services;
using ButtonBehaviourDemo.Events;
using System;

public abstract class BaseService : IService
{
    protected readonly EventBus _bus; // EventBus shared between all services.
    private readonly ActionQueue _queue = new(); // Separate ActionQueue for each service to handle its own events.
    private int _intervalMs; // Interval in milliseconds for periodic service execution.
    private CancellationTokenSource _cts; // Cancellation token source to manage the service lifecycle.

    protected BaseService(EventBus bus)
    {
        _bus = bus;
    }

    public void HandleEvents() => _queue.Process(); // Process all events in the queue. This method is public for testing purposes.

    public virtual void Service() { } // Hook for any logic that needs to be executed periodically. Public for testing purposes.

    // Subscribe to events of type T. Messages should be processed by the parsed handler.
    protected void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        _bus.Subscribe(handler, _queue);
    }

    // Publish an event of type T. This will notify all subscribers of the event.
    protected void Publish<T>(T evt) where T : IEvent
    {
        _bus.Publish(evt);
    }

    // Start service in a separate thread, processing events at the specified interval.
    public void Start(int intervalMs)
    {
        _intervalMs = intervalMs;
        _cts = new CancellationTokenSource();

        Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                HandleEvents(); // Handle any events in the queue.
                Service(); // Call the periodic service method. TODO: Should we have hooks pre and post event handling?
                await Task.Delay(_intervalMs, _cts.Token);
            }
        });
    }

    // Stop service.
    public void Stop()
    {
        _cts?.Cancel();
    }
}
