using Godot;

[GlobalClass]
public abstract partial class TimedEquipment : Equipment
{
    [Export]
    public float secondsBetweenProgressChecks { get; private set; } = 1f;

    [Export(PropertyHint.Range, "100,150,1,or_greater,prefer_slider")]
    public int percentFailureThreshold { get; private set; } = 120;
}
