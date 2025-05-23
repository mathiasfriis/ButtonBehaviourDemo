using ButtonBehaviourDemo.Events;
using ButtonBehaviourDemo.Services;
using ButtonBehaviourDemo.Events;
using System;

public abstract class BaseService : IService
{
    protected readonly EventBus _bus;
    private readonly ActionQueue _queue = new();
    private int _intervalMs;
    private CancellationTokenSource _cts;

    protected BaseService(EventBus bus)
    {
        _bus = bus;
    }

    public void HandleEvents() => _queue.Process();

    protected virtual void Service() { } // Hook for any logic that needs to be executed periodically.

    protected void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        _bus.Subscribe(handler, _queue);
    }

    protected void Publish<T>(T evt) where T : IEvent
    {
        _bus.Publish(evt);
    }

    public void Start(int intervalMs)
    {
        _intervalMs = intervalMs;
        _cts = new CancellationTokenSource();

        Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                HandleEvents();
                Service(); // Call the periodic service method.
                await Task.Delay(_intervalMs, _cts.Token);
            }
        });
    }

    public void Stop()
    {
        _cts?.Cancel();
    }
}
