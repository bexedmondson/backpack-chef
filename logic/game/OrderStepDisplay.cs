using Godot;

public partial class OrderStepDisplay : Control
{
    [Export]
    private Label stepNumberLabel;
    [Export]
    private TextureRect stepEquipmentTextureRect;
    [Export]
    private Control checkmarkIcon;

    private OrderStep step;

    public void SetStep(int stepNumber, OrderStep orderStep)
    {
        step = orderStep;
        stepNumberLabel.Text = (stepNumber + 1).ToString();
        stepEquipmentTextureRect.Texture = step.equipment.smallIcon;
        checkmarkIcon.Visible = false; //TODO update progress
    }
}
