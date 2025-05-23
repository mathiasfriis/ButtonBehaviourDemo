using ButtonBehaviourDemo.Events;
using System;
using System.Threading.Tasks;


namespace ButtonBehaviourDemo.Services
{
    public class OrderService
    {
        private readonly EventBus _bus;
        public OrderService(EventBus bus)
        {
            _bus = bus;
            _bus.Subscribe<UserCreatedEvent>(HandleUserCreated);
        }

        private Task HandleUserCreated(UserCreatedEvent evt)
        {
            Console.WriteLine($"[OrderService] Detected new user: {evt.Username}. Placing order...");
            return _bus.Publish(new OrderPlacedEvent { OrderId = new Random().Next(1000, 9999) });
        }
    }
}