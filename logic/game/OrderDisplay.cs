using Godot;

public partial class OrderDisplay : Control
{
    [Export]
    private Label recipeNameLabel;
    
    public Order order { get; private set; }

    public void Setup(Order newOrder)
    {
        this.order = newOrder;
        recipeNameLabel.Text = order.recipe.name;
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
