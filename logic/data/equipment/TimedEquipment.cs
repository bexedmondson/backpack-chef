using Godot;

[GlobalClass]
public abstract partial class TimedEquipment : Equipment
{
    [Export]
    public float secondsBetweenProgressChecks { get; private set; } = 1f;

    [Export(PropertyHint.Range, "100,300,1,or_greater,prefer_slider")]
    public int percentFailureThreshold { get; private set; } = 200;
    
    [Export(PropertyHint.Range, "100,300,1,or_greater,prefer_slider")]
    public int percentWarningThreshold { get; private set; } = 150;

    public override bool HasOrderStepFailed(OrderStep step, double progressPercent)
    {
        return progressPercent >= percentFailureThreshold;
    }
}
