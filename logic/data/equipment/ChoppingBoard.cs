using Godot;

[GlobalClass]
public partial class ChoppingBoard : Equipment
{
    public override bool HasOrderStepFailed(OrderStep step, double progress)
    {
        return false;
    }
}
