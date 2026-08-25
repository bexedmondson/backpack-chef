using Godot;

public abstract partial class EquipmentDisplay : Control
{
    [Export]
    private Control orderDisplayContainer;

    [Export]
    private Node currentOrderStepVisualOverlayParent;

    protected OrderManager orderManager;
    protected Equipment equipment;
    protected OrderDisplay currentOrderDisplay;
    protected Control currentOrderStepVisualOverlay;

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

    public void SetEquipment(Equipment e)
    {
        equipment = e;
        equipment.OnChange += RefreshCurrentOrderDisplay;
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

        Log.Print(this.Name, true);
        
        currentOrderDisplay = orderDisplay;
        orderDisplay.Reparent(orderDisplayContainer); //should we move this to the refresh display maybe??

        Log.Print($"{this.Name} order display obtained", true);
        
        //TODO this should update some kind of manager, which should then update the visuals of everything
        //because we will eventually want to save which order is at which equipment
        
        Log.Print($"{this.Name} order {orderDisplay.order.recipe.name} moving to equipment", true);
        orderManager.OnOrderMovedToEquipment(this.equipment, orderDisplay.order);
        Log.Print($"{this.Name} order {orderDisplay.order.recipe.name} moved to equipment", true);
        
        Log.Print($"{this.Name} setting {equipment.name} current order", true);
        equipment.SetCurrentOrder(orderDisplay.order);
    }

    private void RefreshCurrentOrderDisplay()
    {
        var currentOrderAtEquipment = equipment.currentOrder;
        if (currentOrderAtEquipment == null)
        {
            if (currentOrderStepVisualOverlay != null)
            {
                currentOrderStepVisualOverlay.QueueFree();
                currentOrderStepVisualOverlay = null;
            }
            currentOrderDisplay = null;
            return;
        }

        var currentStep = equipment.currentOrder.currentStep;

        if (currentOrderStepVisualOverlay == null)
        {
            currentOrderStepVisualOverlay = currentStep.GetVisualOverlayScene().Instantiate<Control>();
            currentOrderStepVisualOverlayParent.AddChild(currentOrderStepVisualOverlay);
        }

        if (currentStep.isStepFinished)
        {
            if (currentStep.didStepFail)
                currentOrderStepVisualOverlay.Modulate = Colors.DimGray;
            else
            {
                //TODO not the greatest way to swap overlays but not bad for now
                currentOrderStepVisualOverlay.QueueFree();
                currentOrderStepVisualOverlay = currentStep.GetVisualOverlayScene().Instantiate<Control>();
                currentOrderStepVisualOverlayParent.AddChild(currentOrderStepVisualOverlay);
            }
        }
    }

    public void TryMakeProgress()
    {
        if (CanMakeProgress())
            MakeProgress();
    }

    protected virtual bool CanMakeProgress()
    {
        if (currentOrderDisplay?.order == null)
            return false;
        return true;
    }

    private void MakeProgress()
    {
        currentOrderDisplay.order.currentStep.MakeProgress(50);
    }
    
    public Order GetCurrentDisplayedOrder()
    {
        if (currentOrderDisplay == null)
            return null;
        return currentOrderDisplay.order;
    }
}
