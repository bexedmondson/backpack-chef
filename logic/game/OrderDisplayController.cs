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
        Injection.Get<EventDispatcher>().Add<OrderStateChangedEvent>(OnOrderStateChanged);
    }

    private void OnOrderCreated(OrderCreatedEvent orderCreatedEvent)
    {
        //placeholder is positioned in order "queue" display, so will automatically add to the queue area
        var newOrderDisplay = orderDisplayPlaceholder.CreateInstance() as OrderDisplay;
        newOrderDisplay.Setup(orderCreatedEvent.order);
        orderDisplays.Add(newOrderDisplay);
    }

    private void OnOrderStateChanged(OrderStateChangedEvent orderStateChangedEvent)
    {
        //no need to do anything to the order displays, we'll keep the reference here so that it can be cleaned up when this scene goes away
        //by this controller instead of passing that responsibility around to equipment displays and such
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Injection.Get<EventDispatcher>().Remove<OrderCreatedEvent>(OnOrderCreated);
        Injection.Get<EventDispatcher>().Remove<OrderStateChangedEvent>(OnOrderStateChanged);
        
        foreach (var orderDisplay in orderDisplays)
        {
            orderDisplay.QueueFree();
        }
        orderDisplays.Clear();
    }
}
