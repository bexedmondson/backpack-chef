using Godot;

[GlobalClass]
public partial class Plate : Equipment
{
    public override bool HasOrderStepFailed(OrderStep step, double progress)
    {
        return false;
    }
}
