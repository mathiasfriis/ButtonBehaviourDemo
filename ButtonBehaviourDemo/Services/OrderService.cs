using ButtonBehaviourDemo.Events;

public class OrderService : BaseService
{
    public OrderService(EventBus bus) : base(bus)
    {
        Subscribe<UserCreatedEvent>(HandleUserCreated);
    }

    private void HandleUserCreated(UserCreatedEvent evt)
    {
        Console.WriteLine($"[OrderService] Detected new user: {evt.Username}. Placing order...");
        Publish(new OrderPlacedEvent { OrderId = new Random().Next(1000, 9999) });
    }
}
