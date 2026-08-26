public partial class ChoppingBoardDisplay : EquipmentDisplay
{
    protected override bool CanMakeProgress()
    {
        if (!base.CanMakeProgress())
            return false;

        if (equipment.currentOrder.currentStep.isStepFinished)
            return false;
        return true;
    }

    protected override void MakeProgress()
    {
        currentOrderDisplay.order.currentStep.MakeProgress(10);
    }
}
