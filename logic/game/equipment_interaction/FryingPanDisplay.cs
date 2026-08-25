public partial class FryingPanDisplay : EquipmentDisplay
{
    protected override bool CanMakeProgress()
    {
        if (!base.CanMakeProgress())
            return false;
        
        if (currentOrderDisplay.order.currentStep.didStepFail)
            return false;

        return true;
    }
}
