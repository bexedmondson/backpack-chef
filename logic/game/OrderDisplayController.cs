using System.Collections.Generic;
using Godot;

public partial class OrderDisplayController : Node
{
    [Export]
    public InstancePlaceholder orderDisplayPlaceholder;

    private OrderManager orderManager;
    private List<OrderDisplay> orderDisplays = new(); 

    public override void _EnterTree()
    {
        base._EnterTree();
        orderManager = Injection.Get<OrderManager>();

        Injection.Get<EventDispatcher>().Add<OrderCreatedEvent>(OnOrderCreated);
    }

    private void OnOrderCreated(OrderCreatedEvent orderCreatedEvent)
    {
        var newOrderDisplay = orderDisplayPlaceholder.CreateInstance() as OrderDisplay;
        newOrderDisplay.Setup(orderCreatedEvent.order);
        orderDisplays.Add(newOrderDisplay);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Injection.Get<EventDispatcher>().Remove<OrderCreatedEvent>(OnOrderCreated);
        
        foreach (var orderDisplay in orderDisplays)
        {
            orderDisplay.QueueFree();
        }
        orderDisplays.Clear();
    }
}
