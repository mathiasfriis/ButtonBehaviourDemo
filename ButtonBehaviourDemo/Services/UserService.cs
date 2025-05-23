using ButtonBehaviourDemo.Events;
using System;
using System.Threading.Tasks;

namespace ButtonBehaviourDemo.Services
{
    public class UserService
    {
        private readonly EventBus _bus;
        public UserService(EventBus bus)
        {
            _bus = bus;
            _bus.Subscribe<OrderPlacedEvent>(HandleOrderPlaced);
        }

        private Task HandleOrderPlaced(OrderPlacedEvent evt)
        {
            Console.WriteLine($"[UserService] Received order ID {evt.OrderId}");
            return Task.CompletedTask;
        }

        public async Task CreateUser(string username)
        {
            Console.WriteLine($"[UserService] Creating user: {username}");
            await _bus.Publish(new UserCreatedEvent { Username = username });
        }
    }
}
