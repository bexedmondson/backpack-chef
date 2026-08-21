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
}
