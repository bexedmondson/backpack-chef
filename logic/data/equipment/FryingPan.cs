using Godot;

[GlobalClass]
public partial class FryingPan : TimedEquipment
{
    [Export]
    public int sliderMax { get; private set; } = 20;
    
    [Export]
    public int sliderOptimal { get; private set; } = 10;
    
    [Export]
    public int sliderOkayDistanceFromOptimal { get; private set; } = 5;

    public override double GetProgressPercent(double input = 0)
    {
        //input for frying pan should be slider value
        if (input > sliderOptimal + sliderOkayDistanceFromOptimal) //order is burning!
            return 0;

        if (input < sliderOptimal - sliderOkayDistanceFromOptimal)
            return 0;

        var distanceFromOptimal = Mathf.Abs(input - sliderOptimal);
        //at extreme edges of okay range, order cooks at 50% speed; increases linearly to 100% at optimal
        return defaultProgress * 0.5f + (1f - distanceFromOptimal/sliderOkayDistanceFromOptimal) * 0.5f; 
    }
}
