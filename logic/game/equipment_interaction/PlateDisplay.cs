public partial class PlateDisplay : EquipmentDisplay
{
    protected override void MakeProgress()
    {
        equipment.ProgressCurrentOrder(equipment.GetProgressPercentDelta());
    }
}
