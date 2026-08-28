using Godot;

[GlobalClass]
public partial class Plate : Equipment
{
    public override bool HasOrderStepFailed(OrderStep step, double progress)
    {
        return false;
    }

    public override void ProgressCurrentOrder(double percentIncrease)
    {
        base.ProgressCurrentOrder(percentIncrease);

        var hasOrder = Injection.Get<EquipmentManager>().TryGetOrder(this, out var order);

        if (hasOrder)
            Injection.Get<OrderManager>().ServeCompleteOrder(order);
    }
}
