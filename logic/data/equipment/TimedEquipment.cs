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

    public override bool IsOverheating()
    {
        equipmentManager ??= Injection.Get<EquipmentManager>();
        var hasOrder = equipmentManager.TryGetOrder(this, out var order);

        if (!hasOrder)
            return false;

        var progress = order.currentStep.progressPercent;
        Log.Print($"Timed equipment progress percent is {progress} and warning threshold is {percentWarningThreshold}");
        return progress >= percentWarningThreshold;
    }
}
