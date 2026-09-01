using Godot;

public partial class FryingPanDisplay : EquipmentDisplay
{
    [Export]
    private Slider slider;

    [Export]
    private Control sliderFill;

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
        var hasOrder = equipmentManager.TryGetOrder(equipment, out var currentOrder);
        if (!hasOrder)
        {
            timeSinceLastProgressCheck = 0;
            return;
        }
        
        var timedEquipment = equipment as TimedEquipment;

        if (currentOrder.currentStep.progressPercent == 0) //proxy for finding the point where a new step is started
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

    protected override bool CanTryMakingProgress()
    {
        if (!base.CanTryMakingProgress())
            return false;

        if (equipment is not TimedEquipment timedEquipment)
            return false;

        if (!equipmentManager.TryGetOrder(equipment, out var order))
            return false;
        
        if (order.currentStep.didStepFail)
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

        if (!equipmentManager.HasOrder(equipment))
            return;
        
        equipment.ProgressCurrentOrder(equipment.GetProgressPercent(slider.Value));
    }


    public override void RefreshCurrentOrderDisplay()
    {
        base.RefreshCurrentOrderDisplay();

        sliderFill.SetInstanceShaderParameter("instance_shader_parameters/target_area_min_value", minSliderValueForProgress);
        sliderFill.SetInstanceShaderParameter("instance_shader_parameters/target_area_max_value", maxSliderValueForProgress);
        sliderFill.SetInstanceShaderParameter("instance_shader_parameters/bar_max_value", slider.MaxValue);
    }
}
