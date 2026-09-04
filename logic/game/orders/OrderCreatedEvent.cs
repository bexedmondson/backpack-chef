public class OrderCreatedEvent(Order order) : IEvent
{
    public Order order = order;
}
