using Godot;

public partial class FryingPanDisplay : EquipmentDisplay
{
    [Export]
    private Slider slider;

    private int minSliderValueForProgress = 5;
    private int maxSliderValueForProgress = 15;

    private double timeSinceLastProgressCheck = 0f;

    public override void SetEquipment(Equipment e)
    {
        base.SetEquipment(e);
        equipment.OnChange += OnEquipmentChange;
    }

    private void OnEquipmentChange()
    {
        var timedEquipment = equipment as TimedEquipment;
        if (timedEquipment?.currentOrder == null)
        {
            timeSinceLastProgressCheck = 0;
            return;
        }

        if (timedEquipment.currentOrder.currentStep.progressPercent == 0) //proxy for finding the point where a new step is started
        {
            timeSinceLastProgressCheck = timedEquipment.secondsBetweenProgressChecks;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        timeSinceLastProgressCheck += delta;
        TryMakeProgress();
    }

    protected override bool CanMakeProgress()
    {
        if (!base.CanMakeProgress())
            return false;
        
        if (equipment.currentOrder.currentStep.didStepFail)
            return false;

        if (equipment is not TimedEquipment timedEquipment)
            return false;
        
        if (timeSinceLastProgressCheck < timedEquipment.secondsBetweenProgressChecks)
            return false;
        
        return true;
    }

    protected override void MakeProgress()
    {
        if (slider.Value < minSliderValueForProgress)
            return;
        if (slider.Value > maxSliderValueForProgress)
        {
            //DO SOMETHING HERE
            return;
        }
        
        //add a timer between progress increases here
        equipment.currentOrder.currentStep.MakeProgress(10);
    }
}
