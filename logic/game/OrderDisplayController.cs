using System.Collections.Generic;
using Godot;

public partial class OrderDisplayController : Node, IInjectable
{
    [Export]
    public InstancePlaceholder orderDisplayPlaceholder;

    private OrderManager orderManager;
    private Dictionary<Order, OrderDisplay> orderToDisplayMap = new();
    private Dictionary<OrderDisplay, Order> displayToOrderMap = new();

    public override void _EnterTree()
    {
        base._EnterTree();
        Injection.Register(this);
        orderManager = Injection.Get<OrderManager>();

        Injection.Get<EventDispatcher>().Add<OrderCreatedEvent>(OnOrderCreated);
        Injection.Get<EventDispatcher>().Add<OrderStateChangedEvent>(OnOrderStateChanged);
    }

    private void OnOrderCreated(OrderCreatedEvent orderCreatedEvent)
    {
        //placeholder is positioned in order "queue" display, so will automatically add to the queue area
        var newOrderDisplay = orderDisplayPlaceholder.CreateInstance() as OrderDisplay;
        newOrderDisplay.Setup(orderCreatedEvent.order);
        orderToDisplayMap[orderCreatedEvent.order] = newOrderDisplay;
        displayToOrderMap[newOrderDisplay] = orderCreatedEvent.order;
    }

    private void OnOrderStateChanged(OrderStateChangedEvent orderStateChangedEvent)
    {
        //no need to do anything to the order displays, we'll keep the reference here so that it can be cleaned up when this scene goes away
        //by this controller instead of passing that responsibility around to equipment displays and such
    }

    public void OnOrderBinned(Order order)
    {
        var display = orderToDisplayMap[order];

        if (display == null)
        {
            Log.Warn($"Display for binned order {order.recipe.name} is null! Returning??");
            return;
        }
        
        display.Reparent(orderDisplayPlaceholder.GetParent());
    }

    public void OnOrderEnded(Order order)
    {
        var display = orderToDisplayMap[order];

        if (display == null)
        {
            Log.Warn($"Display for ended order {order.recipe.name} is null - probably already disposed. Returning.");
            return;
        }
        
        display.DoOrderRemovalAnimation(() =>
        {
            RemoveOrderDisplayForOrder(order, display);
        });
    }

    private void RemoveOrderDisplayForOrder(Order order, OrderDisplay display)
    {
        display.QueueFree();
        orderToDisplayMap[order] = null;
    }
    
    public Order GetOrderForDisplay(OrderDisplay orderDisplay)
    {
        return displayToOrderMap.GetValueOrDefault(orderDisplay);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Injection.Deregister(this);
        Injection.Get<EventDispatcher>().Remove<OrderCreatedEvent>(OnOrderCreated);
        Injection.Get<EventDispatcher>().Remove<OrderStateChangedEvent>(OnOrderStateChanged);
        
        foreach (var kvp in orderToDisplayMap)
        {
            kvp.Value.QueueFree();
        }
        orderToDisplayMap.Clear();
        displayToOrderMap.Clear();
        orderManager = null;
    }
}
