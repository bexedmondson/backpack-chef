public partial class ChoppingBoardDisplay : EquipmentDisplay
{
    protected override bool CanMakeProgress()
    {
        if (!base.CanMakeProgress())
            return false;

        if (currentOrderDisplay.order.currentStep.isStepFinished)
            return false;
        return true;
    }
}
