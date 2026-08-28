public partial class ChoppingBoardDisplay : EquipmentDisplay
{
    protected override bool CanTryMakingProgress()
    {
        if (!base.CanTryMakingProgress())
            return false;

        var hasOrder = equipmentManager.TryGetOrder(equipment, out var currentOrder);
        if (currentOrder.currentStep.isStepFinished)
            return false;
        return true;
    }

    protected override void MakeProgress()
    {
        equipment.ProgressCurrentOrder(equipment.GetProgressPercent());
    }
}
