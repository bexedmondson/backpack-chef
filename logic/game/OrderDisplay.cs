using Godot;

public partial class OrderDisplay : Control
{
    [Export]
    private Label recipeNameLabel;
    
    [Export]
    private TextureRect recipeIcon;

    [Export]
    private InstancePlaceholder stepDisplayPlaceholder;

    private Order order;

    public void Setup(Order newOrder)
    {
        this.order = newOrder;
        recipeNameLabel.Text = GameDebug.On ? order.recipe.name : string.Empty;
        
        recipeIcon.Texture = order.recipe.icon;

        for (int i = 0; i < order.steps.Length; i++)
        {
            var step = order.steps[i];
            var newStepDisplay = stepDisplayPlaceholder.CreateInstance() as OrderStepDisplay;
            newStepDisplay.SetStep(i, step);
        }
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        SetDragPreview(GetDragPreview());
        return this;
    }

    private Control GetDragPreview()
    {
        var dupe = this.Duplicate() as Control;

        dupe.Size = this.Size;
        
        this.Modulate = Colors.Transparent;
        return dupe;
    }
    
    public override void _Notification(int what)
    {
        if (what == NotificationDragEnd)
        {
            this.Modulate = Colors.White;
        }
    }
}
