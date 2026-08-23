using Godot;

public partial class EquipmentDisplay : Control
{
    [Export]
    private TextureRect equipmentTextureRect;

    [Export]
    private Control orderDisplayContainer;

    private Equipment equipment;
    private OrderDisplay currentOrderDisplay;

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
        return currentOrderDisplay == null;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        base._DropData(atPosition, data);
        if (data.As<GodotObject>() is not OrderDisplay orderDisplay)
            return;

        Injection.Get<OrderManager>().OnOrderMovedToEquipment(this.equipment, orderDisplay.order);
        
        currentOrderDisplay = orderDisplay;
        orderDisplay.Reparent(orderDisplayContainer);
    }

    public void RefreshCurrentOrderDisplay()
    {
        if (currentOrderDisplay.GetParent() != orderDisplayContainer)
            currentOrderDisplay = null;
    }
    
    public Order GetCurrentDisplayedOrder()
    {
        if (currentOrderDisplay == null)
            return null;
        return currentOrderDisplay.order;
    }
}
