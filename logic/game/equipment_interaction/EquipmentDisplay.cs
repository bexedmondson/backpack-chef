using Godot;

public abstract partial class EquipmentDisplay : Control
{
    [Export]
    private Control orderDisplayContainer;

    [Export]
    private Node currentOrderStepVisualOverlayParent;

    protected OrderManager orderManager;

    protected EquipmentManager equipmentManager;
    
    protected Equipment equipment;
    protected OrderDisplay currentOrderDisplay;
    protected Control currentOrderStepVisualOverlay;
    
    public override void _EnterTree()
    {
        base._EnterTree();
        orderManager = Injection.Get<OrderManager>();
        equipmentManager = Injection.Get<EquipmentManager>();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        orderManager = null;
        equipmentManager = null;
    }

    public virtual void SetEquipment(Equipment e)
    {
        equipment = e;
        equipment.OnChange += RefreshCurrentOrderDisplay;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        if (data.As<GodotObject>() is not OrderDisplay orderDisplay)
            return false;

        return equipmentManager.CanOrderMoveToEquipment(orderDisplay.order, equipment);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        base._DropData(atPosition, data);
        if (data.As<GodotObject>() is not OrderDisplay orderDisplay)
            return;

        Log.Print(this.Name, true);
        
        currentOrderDisplay = orderDisplay;
        orderDisplay.Reparent(orderDisplayContainer);
        
        equipmentManager.MoveOrderToEquipment(orderDisplay.order, equipment);

        /*Log.Print($"{this.Name} order display obtained", true);
        Log.Print($"{this.Name} order {orderDisplay.order.recipe.name} moving to equipment", true);
        orderManager.OnOrderMovedToEquipment(this.equipment, orderDisplay.order);
        Log.Print($"{this.Name} order {orderDisplay.order.recipe.name} moved to equipment", true);

        Log.Print($"{this.Name} setting {equipment.name} current order", true);
        equipment.SetCurrentOrder(orderDisplay.order);*/
    }

    public void RefreshCurrentOrderDisplay()
    {
        var hasOrder = equipmentManager.TryGetOrder(this.equipment, out var currentOrderAtEquipment);
        if (!hasOrder)
        {
            if (currentOrderStepVisualOverlay != null)
            {
                currentOrderStepVisualOverlay.QueueFree();
                currentOrderStepVisualOverlay = null;
            }
            currentOrderDisplay = null;
            return;
        }

        var currentStep = currentOrderAtEquipment.currentStep;

        if (currentOrderStepVisualOverlay == null)
        {
            currentOrderStepVisualOverlay = currentStep.GetVisualOverlayScene().Instantiate<Control>();
            currentOrderStepVisualOverlayParent.AddChild(currentOrderStepVisualOverlay);
            return;
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
        if (CanTryMakingProgress())
            MakeProgress();
    }

    protected virtual bool CanTryMakingProgress()
    {
        return equipmentManager.HasOrder(equipment);
    }

    protected virtual void MakeProgress()
    {
        //equipment.currentOrder.currentStep.MakeProgress(equipment.GetProgressPercent());
    }
}
