using Godot;

public partial class EquipmentSlot : Control
{
    [Export]
    private TextureRect equipmentIcon;
    [Export]
    private TextureRect orderDisplayContainer;

    private Order currentOrder = null;

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        //TODO
        return base._CanDropData(atPosition, data);
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        base._DropData(atPosition, data);
        if (data.As<GodotObject>() is OrderDisplay orderDisplay)
        {
            currentOrder = orderDisplay.order;
            orderDisplay.Reparent(orderDisplayContainer);
        }
    }
}
