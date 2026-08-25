using Godot;

public partial class EquipmentDisplay : Control
{
    [Export]
    private TextureRect equipmentTextureRect;

    [Export]
    private Control orderDisplayContainer;

    [Export]
    private Node currentOrderStepVisualOverlayParent;

    private OrderManager orderManager;
    private Equipment equipment;
    private OrderDisplay currentOrderDisplay;
    private Control currentOrderStepVisualOverlay;

    public override void _EnterTree()
    {
        base._EnterTree();
        orderManager = Injection.Get<OrderManager>();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        orderManager = null;
    }

    public override void _Ready()
    {
        base._Ready();
        orderDisplayContainer.ChildOrderChanged += RefreshCurrentOrderDisplay;
    }

    public void SetEquipment(Equipment e)
    {
        equipment = e;
        equipmentTextureRect.Texture = equipment.icon;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if (currentOrderDisplay != null)
            return false;
        
        if (data.As<GodotObject>() is not OrderDisplay orderDisplay)
            return false;

        return orderManager.CanOrderMoveToEquipment(orderDisplay.order, equipment);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        base._DropData(atPosition, data);
        if (data.As<GodotObject>() is not OrderDisplay orderDisplay)
            return;

        Injection.Get<OrderManager>().OnOrderMovedToEquipment(this.equipment, orderDisplay.order);
        
        currentOrderDisplay = orderDisplay;
        orderDisplay.Reparent(orderDisplayContainer);

        currentOrderStepVisualOverlay = orderDisplay.order.currentStep.visualOverlayStepStart.Instantiate<Control>();
        currentOrderStepVisualOverlayParent.AddChild(currentOrderStepVisualOverlay);

        currentOrderDisplay.order.currentStep.OnStepCompleted += RefreshCurrentOrderDisplay;
    }

    private void RefreshCurrentOrderDisplay()
    {
        RefreshCurrentOrderDisplay(null); //TODO kind of hate this. find a better way to do it!
    }
    
    private void RefreshCurrentOrderDisplay(OrderStep currentStep)
    {
        if (currentOrderDisplay.GetParent() != orderDisplayContainer)
        {
            currentOrderDisplay = null;
            if (currentOrderStepVisualOverlay != null)
                currentOrderStepVisualOverlay.QueueFree();
        }
        else if (currentStep != null && currentStep.isStepFinished && currentOrderStepVisualOverlay != null)
        {
            if (currentStep.didStepFail)
                currentOrderStepVisualOverlay.Modulate = Colors.DimGray;
            else
            {
                currentOrderStepVisualOverlay.QueueFree();
                currentOrderStepVisualOverlay = currentStep.visualOverlayStepEnd.Instantiate<Control>();
                currentOrderStepVisualOverlayParent.AddChild(currentOrderStepVisualOverlay);
            }
        }
    }
    
    public Order GetCurrentDisplayedOrder()
    {
        if (currentOrderDisplay == null)
            return null;
        return currentOrderDisplay.order;
    }
}
