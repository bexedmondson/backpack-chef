public class OrderStateChangedEvent(Order order) : IEvent
{
    public Order order = order;
}
