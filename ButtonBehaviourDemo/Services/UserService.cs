using ButtonBehaviourDemo.Events;

public class UserService : BaseService
{
    public UserService(EventBus bus) : base(bus)
    {
        Subscribe<OrderPlacedEvent>(HandleOrderPlaced);
    }

    private void HandleOrderPlaced(OrderPlacedEvent evt)
    {
        Console.WriteLine($"[UserService] Received order ID {evt.OrderId}");
    }

    public void CreateUser(string username)
    {
        Console.WriteLine($"[UserService] Creating user: {username}");
        Publish(new UserCreatedEvent { Username = username });
    }
}
