using Godot;

public partial class ChoppingBoardDisplay : EquipmentDisplay
{
    [Export]
    private Button button;
    
    [Export]
    private Control knifeDownTextureRect;
    
    [Export]
    private Control knifeUpTextureRect;

    public override void _EnterTree()
    {
        base._EnterTree();
        button.ButtonDown += OnButtonDown;
        button.ButtonUp += OnButtonUp;

        OnButtonUp();
    }

    protected override bool CanTryMakingProgress()
    {
        if (!base.CanTryMakingProgress())
            return false;

        var hasOrder = equipmentManager.TryGetOrder(equipment, out var currentOrder);
        if (hasOrder && currentOrder.currentStep.isStepFinished)
            return false;
        return true;
    }

    protected override void MakeProgress()
    {
        equipment.ProgressCurrentOrder(equipment.GetProgressPercentDelta());
    }

    private void OnButtonDown()
    {
        knifeDownTextureRect.Visible = true;
        knifeUpTextureRect.Visible = false;
    }

    private void OnButtonUp()
    {
        knifeDownTextureRect.Visible = false;
        knifeUpTextureRect.Visible = true;
    }
}
